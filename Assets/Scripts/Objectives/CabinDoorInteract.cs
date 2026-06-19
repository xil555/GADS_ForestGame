using UnityEngine;

/// <summary>
/// Press E near the cabin door to open or close. Rotates a child leaf (typical asset-store cabin setup).
/// Add a trigger collider on this object or the door frame.
/// </summary>
[DefaultExecutionOrder(500)]
[RequireComponent(typeof(Collider))]
public class CabinDoorInteract : MonoBehaviour
{
    [SerializeField] Transform doorLeaf;
    [SerializeField] float openAngleY = 95f;
    [SerializeField] float rotateSpeed = 4f;
    [SerializeField] float interactionRange = 2.5f;
    [SerializeField] bool openOutward = true;

    bool _playerNear;
    bool _isOpen;
    float _currentAngle;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void Awake()
    {
        if (doorLeaf == null && transform.childCount > 0)
            doorLeaf = transform.GetChild(0);
    }

    void Update()
    {
        float target = _isOpen ? (openOutward ? openAngleY : -openAngleY) : 0f;
        _currentAngle = Mathf.MoveTowards(_currentAngle, target, rotateSpeed * 90f * Time.deltaTime);

        if (doorLeaf != null)
            doorLeaf.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);

        if (!IsPlayerInRange())
            return;

        if (Input.GetKeyDown(KeyCode.E))
            _isOpen = !_isOpen;
    }

    void LateUpdate()
    {
        if (!IsPlayerInRange())
            return;

        // Pass a literal integer value if your PromptManager handles priorities as ints
        PromptManager.SubmitPromptCandidate("Press E to interact", 1);
    }

    bool IsPlayerInRange()
    {
        if (_playerNear)
            return true;

        Vector3 point = doorLeaf != null ? doorLeaf.position : transform.position;
        return InteractionRangeHelper.IsPlayerWithinRange(point, interactionRange);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _playerNear = false;
    }
}
