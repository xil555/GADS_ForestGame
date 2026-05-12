using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FenceRepair : MonoBehaviour
{
    [SerializeField] GameObject repairedFence;
    [SerializeField] GameObject brokenFenceVisual;

    bool playerNear;
    float holdTime;
    bool completedThisCycle;

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

        var col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        // Repair may have turned off sibling meshes under this segment; restore before applying day state.
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);

        if (repairedFence != null)
            repairedFence.SetActive(false);

        if (brokenFenceVisual != null)
            brokenFenceVisual.SetActive(true);
    }

    void Awake()
    {
        if (brokenFenceVisual == null)
            brokenFenceVisual = gameObject;
    }

    void Update()
    {
        if (!playerNear || completedThisCycle)
            return;

        if (ObjectiveManager.Instance == null)
            return;

        if (!ObjectiveManager.Instance.HasObjective(ObjectiveType.Fence))
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

                if (repairedFence != null)
                    repairedFence.SetActive(true);

                HideBrokenVisualsLeavingRepaired();

                completedThisCycle = true;

                // If the fixed mesh lives under this object, do not disable the root or the repair vanishes / leaves gaps.
                bool repairedIsChildOfThis = repairedFence != null && repairedFence.transform.IsChildOf(transform);
                if (repairedIsChildOfThis)
                {
                    var col = GetComponent<Collider>();
                    if (col != null)
                        col.enabled = false;
                    enabled = false;
                }
                else
                    gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
            holdTime = 0f;
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

    void HideBrokenVisualsLeavingRepaired()
    {
        if (brokenFenceVisual == null || brokenFenceVisual == repairedFence)
            return;

        if (brokenFenceVisual != gameObject)
        {
            brokenFenceVisual.SetActive(false);
            return;
        }

        // Script default used the root as "broken"; repaired is a child — hide sibling meshes only.
        if (repairedFence == null || !repairedFence.transform.IsChildOf(transform))
        {
            brokenFenceVisual.SetActive(false);
            return;
        }

        foreach (Transform child in transform)
        {
            GameObject c = child.gameObject;
            if (c == repairedFence)
                continue;
            if (repairedFence.transform.IsChildOf(child))
                continue;
            c.SetActive(false);
        }
    }
}
