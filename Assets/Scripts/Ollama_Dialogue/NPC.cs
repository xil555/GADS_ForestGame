using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPC : MonoBehaviour
{
    [Header("NPC Details")]
    public string npcName;
    [TextArea(3, 10)] public string systemPrompt;
    public bool isDangerous;

    [Header("Movement & Animation")]
    public float walkSpeed = 2f;
    [HideInInspector] public Transform cabinDoorWaypoint;

    public Animator animator;

    [HideInInspector] public string conversationHistory = "";
    [HideInInspector] public int questionsAsked = 0;
    [HideInInspector] public bool hasStartedConversation = false;
    [HideInInspector] public bool isDeadOrGone = false;

    private bool isAtDoor = false;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (isDeadOrGone) return;

        
        if (!isAtDoor && cabinDoorWaypoint != null)
        {
            if (animator != null) animator.SetBool("isWalking", true);

            transform.position = Vector3.MoveTowards(transform.position, cabinDoorWaypoint.position, walkSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(cabinDoorWaypoint.position.x, transform.position.y, cabinDoorWaypoint.position.z));

            // Check if they arrived at the exact waypoint
            if (Vector3.Distance(transform.position, cabinDoorWaypoint.position) < 0.1f)
            {
                isAtDoor = true;
                if (animator != null) animator.SetBool("isWalking", false);
            }
        }

        
        if (isAtDoor && !gameManager.IsConversing())
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                gameManager.StartConversation(this);
            }
        }
    }

    void LateUpdate()
    {
        if (isDeadOrGone) return;

        
        if (isAtDoor && !gameManager.IsConversing())
        {
            PromptManager.SubmitPromptCandidate($"Press T to talk to {npcName}", 100);
        }
    }

    public string GetSystemPrompt() { return systemPrompt; }
}