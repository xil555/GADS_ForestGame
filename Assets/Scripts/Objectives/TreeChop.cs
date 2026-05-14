using UnityEngine;

/// <summary>
/// Tree chop while the log objective is active. Uses a trigger for proximity, and a camera ray (including trigger colliders)
/// so tall trees still register when you look at the trunk or canopy from a normal standing position.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TreeChop : MonoBehaviour
{
    [SerializeField] float maxChopRaycastDistance = 120f;
    [SerializeField] float maxChopDistanceFromPlayer = 14f;
    [SerializeField] LayerMask chopRaycastLayers = ~0;

    bool playerNear;
    bool choppedThisCycle;
    Transform playerRoot;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    public void ResetForNewDay()
    {
        choppedThisCycle = false;
        gameObject.SetActive(true);
        enabled = true;
    }

    void Update()
    {
        if (choppedThisCycle)
            return;

        if (ObjectiveManager.Instance == null)
            return;

        if (!ObjectiveManager.Instance.HasObjective(ObjectiveType.Logs))
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (!CanAttemptChopThisTree())
            return;

        if (ObjectiveManager.Instance.TryChopTree())
        {
            choppedThisCycle = true;
            gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (choppedThisCycle)
            return;

        if (ObjectiveManager.Instance == null || !ObjectiveManager.Instance.HasObjective(ObjectiveType.Logs))
            return;

        if (CanAttemptChopThisTree())
            PromptManager.SubmitPromptCandidate("Click to chop tree", PromptManager.PriorityTreeChopRay);
    }

    bool CanAttemptChopThisTree()
    {
        if (playerNear)
            return true;

        if (!TryGetPointerRayHitThisTree(out RaycastHit hit))
            return false;

        EnsurePlayerCached();
        if (playerRoot == null)
            return false;

        return Vector3.Distance(playerRoot.position, hit.point) <= maxChopDistanceFromPlayer;
    }

    void EnsurePlayerCached()
    {
        if (playerRoot != null)
            return;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            playerRoot = p.transform;
    }

    bool TryGetPointerRayHitThisTree(out RaycastHit hit)
    {
        hit = default;
        Camera cam = Camera.main;
        if (cam == null)
            return false;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, maxChopRaycastDistance, chopRaycastLayers, QueryTriggerInteraction.Collide))
            return false;

        return hit.collider != null && hit.collider.GetComponentInParent<TreeChop>() == this;
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
