using UnityEngine;

/// <summary>
/// Hold E at a fence segment while the fence objective is active. Only the closest eligible segment
/// receives input so overlapping zones do not fight. Proximity uses trigger and/or distance so
/// CharacterController-only players still register. Repair only swaps visuals on this object —
/// the root is never deactivated so other fence pieces stay in the scene.
/// </summary>
[RequireComponent(typeof(Collider))]
public class FenceRepair : MonoBehaviour
{
    [SerializeField] GameObject repairedFence;
    [SerializeField] GameObject brokenFenceVisual;
    [SerializeField] float interactionRange = 4f;

    static int s_pickFrame = -1;
    static FenceRepair s_pickWinner;

    bool playerNear;
    float holdTime;
    bool completedThisCycle;

    Vector3 InteractionWorldPoint
    {
        get
        {
            var c = GetComponent<Collider>();
            return c != null ? c.bounds.center : transform.position;
        }
    }

    bool EvaluatePlayerInRange()
    {
        return playerNear || InteractionRangeHelper.IsPlayerWithinRange(InteractionWorldPoint, interactionRange);
    }

    public static FenceRepair GetHoldWinnerThisFrame()
    {
        if (Time.frameCount != s_pickFrame)
        {
            s_pickFrame = Time.frameCount;
            s_pickWinner = ComputeClosestEligibleFence();
        }

        return s_pickWinner;
    }

    static FenceRepair ComputeClosestEligibleFence()
    {
        if (ObjectiveManager.Instance == null || !ObjectiveManager.Instance.CanProgressFenceRepair())
            return null;

        if (!InteractionRangeHelper.TryGetPlayerTransform(out Transform pt))
            return null;

        FenceRepair best = null;
        float bestSq = float.MaxValue;

        foreach (var fr in Object.FindObjectsByType<FenceRepair>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (fr == null || !fr.isActiveAndEnabled || !fr.enabled || fr.completedThisCycle)
                continue;

            if (!fr.EvaluatePlayerInRange())
                continue;

            float sq = (fr.InteractionWorldPoint - pt.position).sqrMagnitude;
            if (sq < bestSq)
            {
                bestSq = sq;
                best = fr;
            }
        }

        return best;
    }

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    public void ResetForNewDay()
    {
        completedThisCycle = false;
        holdTime = 0f;
        gameObject.SetActive(true);
        enabled = true;

        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.enabled = true;

        var col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);

        if (repairedFence != null)
            repairedFence.SetActive(false);

        if (brokenFenceVisual != null)
            brokenFenceVisual.SetActive(true);
    }

    void Update()
    {
        if (completedThisCycle)
            return;

        if (ObjectiveManager.Instance == null)
            return;

        if (!ObjectiveManager.Instance.HasObjective(ObjectiveType.Fence))
        {
            holdTime = 0f;
            return;
        }

        if (!ObjectiveManager.Instance.CanProgressFenceRepair())
        {
            holdTime = 0f;
            return;
        }

        if (GetHoldWinnerThisFrame() != this)
        {
            holdTime = 0f;
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            holdTime += Time.deltaTime;

            if (holdTime >= 5f)
            {
                ObjectiveManager.Instance.CompleteFence();
                ApplyRepairedAppearance();

                completedThisCycle = true;

                var col = GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;

                enabled = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
            holdTime = 0f;
    }

    void LateUpdate()
    {
        if (completedThisCycle)
            return;

        if (ObjectiveManager.Instance == null || !ObjectiveManager.Instance.HasObjective(ObjectiveType.Fence))
            return;

        if (!ObjectiveManager.Instance.CanProgressFenceRepair())
            return;

        if (GetHoldWinnerThisFrame() != this)
            return;

        PromptManager.SubmitPromptCandidate("Hold E to repair fence (5 s)", PromptManager.PrioritySurvivalHold);
    }

    /// <summary>Show repaired mesh; hide broken without deactivating this root (other fence segments stay loaded).</summary>
    void ApplyRepairedAppearance()
    {
        if (repairedFence != null)
            repairedFence.SetActive(true);

        if (brokenFenceVisual != null && brokenFenceVisual != repairedFence)
        {
            if (brokenFenceVisual == gameObject)
                SetRenderersEnabledExcludingRepairedBranch(false);
            else
                brokenFenceVisual.SetActive(false);
        }
        else if (repairedFence != null)
            SetRenderersEnabledExcludingRepairedBranch(false);
    }

    void SetRenderersEnabledExcludingRepairedBranch(bool enabled)
    {
        foreach (var r in GetComponentsInChildren<Renderer>(true))
        {
            if (repairedFence != null && r.transform.IsChildOf(repairedFence.transform))
                continue;

            r.enabled = enabled;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNear = false;
        holdTime = 0f;
    }
}
