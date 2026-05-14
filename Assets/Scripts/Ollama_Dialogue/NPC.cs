using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPC : MonoBehaviour
{
    [Header("NPC Details")]
    public string npcName;
    [TextArea(3, 10)] public string systemPrompt;
    public bool isDangerous;

    [Header("Movement")]
    public float walkSpeed = 2f;
    [HideInInspector] public Transform cabinDoorWaypoint;

    //memory holder for the NPC
    [HideInInspector] public string conversationHistory = "";
    [HideInInspector] public int questionsAsked = 0;
    [HideInInspector] public bool hasStartedConversation = false;
    [HideInInspector] public bool isDeadOrGone = false;

    private bool isAtDoor = false;
    private bool playerNear = false;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        GetComponent<Collider>().isTrigger = true; 
    }

    void Update()
    {
        if (isDeadOrGone) return;

        
        if (!isAtDoor && cabinDoorWaypoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, cabinDoorWaypoint.position, walkSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(cabinDoorWaypoint.position.x, transform.position.y, cabinDoorWaypoint.position.z)); // Look where walking

            if (Vector3.Distance(transform.position, cabinDoorWaypoint.position) < 0.1f)
            {
                isAtDoor = true;
            }
        }

        
        if (playerNear && isAtDoor && !gameManager.IsConversing())
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

        
        if (playerNear && isAtDoor && !gameManager.IsConversing())
        {
            PromptManager.SubmitPromptCandidate($"Press T to talk to {npcName}", 100);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerNear = false;
    }

    public string GetSystemPrompt() { return systemPrompt; }
}