using UnityEngine;

/// <summary>
/// Press E to collect while mushroom objective is active. Only the closest eligible mushroom
/// consumes the key so one press does not empty several colliders. Trigger and/or distance
/// proximity so CharacterController setups still work.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MushroomCollect : MonoBehaviour
{
    [SerializeField] float interactionRange = 3f;

    static int s_pickFrame = -1;
    static MushroomCollect s_pickWinner;

    bool playerNear;
    bool collectedThisCycle;

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

    public static MushroomCollect GetPickupWinnerThisFrame()
    {
        if (Time.frameCount != s_pickFrame)
        {
            s_pickFrame = Time.frameCount;
            s_pickWinner = ComputeClosestEligibleMushroom();
        }

        return s_pickWinner;
    }

    static MushroomCollect ComputeClosestEligibleMushroom()
    {
        if (ObjectiveManager.Instance == null || !ObjectiveManager.Instance.CanCollectMoreMushrooms())
            return null;

        if (!InteractionRangeHelper.TryGetPlayerTransform(out Transform pt))
            return null;

        MushroomCollect best = null;
        float bestSq = float.MaxValue;

        foreach (var m in Object.FindObjectsByType<MushroomCollect>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (m == null || !m.isActiveAndEnabled || !m.enabled || m.collectedThisCycle)
                continue;

            if (!m.EvaluatePlayerInRange())
                continue;

            float sq = (m.InteractionWorldPoint - pt.position).sqrMagnitude;
            if (sq < bestSq)
            {
                bestSq = sq;
                best = m;
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
        collectedThisCycle = false;
        gameObject.SetActive(true);
        enabled = true;
    }

    void Update()
    {
        if (collectedThisCycle)
            return;

        if (ObjectiveManager.Instance == null)
            return;

        if (!ObjectiveManager.Instance.CanCollectMoreMushrooms())
            return;

        if (GetPickupWinnerThisFrame() != this)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ObjectiveManager.Instance.TryCollectMushroom())
            {
                collectedThisCycle = true;
                gameObject.SetActive(false);
            }
        }
    }

    void LateUpdate()
    {
        if (collectedThisCycle)
            return;

        if (ObjectiveManager.Instance == null || !ObjectiveManager.Instance.CanCollectMoreMushrooms())
            return;

        if (GetPickupWinnerThisFrame() != this)
            return;

        PromptManager.SubmitPromptCandidate("Press E to collect mushroom", PromptManager.PriorityMushroomCollect);
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
    }
}
