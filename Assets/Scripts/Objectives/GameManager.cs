using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("System References")]
    public OllamaManager ollama;

    [Header("UI References")]
    public TextMeshProUGUI dialogueTextUI;
    public TMP_InputField playerInputField;

    [Header("Interrogation Settings")]
    [Tooltip("How many questions the player can ask before night falls.")]
    public int maxQuestions = 3;

    [Header("Testing References")]
    public NPC testNPC;

    [Header("Player Reference")]
    public FirstPersonController playerController;

    private NPC currentNPC;
    private string conversationHistory = "";
    private bool isConversing = false;
    private int questionsAsked = 0; // counter to track the limit

    FirstPersonController FPS;

    

    void Start()
    {
        playerInputField.gameObject.SetActive(false);
        playerInputField.onSubmit.AddListener(OnPlayerAsksQuestion);
    }

    void Update()
    {
        if (playerInputField.isFocused) return;

        if (Input.GetKeyDown(KeyCode.T) && !isConversing)
        {
            if (testNPC != null) OnNPCArrives(testNPC);
        }

        if (isConversing)
        {
            if (Input.GetKeyDown(KeyCode.Y)) TrustNPC();
            if (Input.GetKeyDown(KeyCode.N)) TurnAwayNPC();
        }
    }

    public void OnNPCArrives(NPC arrivingNPC)
    {
        playerController.enabled = false;
        currentNPC = arrivingNPC;
        isConversing = true;
        questionsAsked = 0; // Reset the counter for the new NPC

        conversationHistory = currentNPC.GetSystemPrompt() +
            "\n\nThe player approaches the door. What is the very first thing you say to them? Keep it to one short sentence.";

        dialogueTextUI.text = "<i>[Generating initial response...]</i>";
        playerInputField.gameObject.SetActive(false);

        ollama.GenerateDialogue(conversationHistory, DisplayDialogue);
    }

    public void OnPlayerAsksQuestion(string question)
    {
        if (string.IsNullOrWhiteSpace(question)) return;

        questionsAsked++; // Add to the counter!

        conversationHistory += "\nPlayer asks: " + question + "\nNPC responds (keep it to one short sentence):";

        dialogueTextUI.text = "<i>[NPC is thinking...]</i>";
        playerInputField.gameObject.SetActive(false);

        ollama.GenerateDialogue(conversationHistory, DisplayDialogue);
    }

    private void DisplayDialogue(string generatedText)
    {
        conversationHistory += " " + generatedText;

        // Check if the player has hit the limit
        if (questionsAsked < maxQuestions)
        {
            // Still have questions left
            dialogueTextUI.text = generatedText + $"\n\n<i>(Question {questionsAsked}/{maxQuestions} used. Type a question or press 'Y'/ 'N')</i>";

            playerInputField.text = "";
            playerInputField.gameObject.SetActive(true);
            playerInputField.ActivateInputField();
        }
        else
        {
            // Out of questions! Force the choice.
            dialogueTextUI.text = generatedText + "\n\n<color=red><i>(Time is up! The sun is setting. You MUST decide: Press 'Y' to Trust, 'N' to Turn Away)</i></color>";
            playerInputField.gameObject.SetActive(false); // Disable input, they can only press Y or N now
        }
    }

    public void TrustNPC()
    {
        Debug.Log("Player pressed Y: Trusted " + currentNPC.npcName);
        DetermineOutcome(true); // true means we trusted them
    }

    public void TurnAwayNPC()
    {
        Debug.Log("Player pressed N: Turned away " + currentNPC.npcName);
        DetermineOutcome(false); // false means we turned them away
    }

    private void DetermineOutcome(bool playerTrustedNPC)
    {
        isConversing = false;
        playerInputField.gameObject.SetActive(false);

        // Let's evaluate the Win/Loss states!
        if (playerTrustedNPC && currentNPC.isDangerous)
        {
            // Trusted the wrong person -> LOSE
            dialogueTextUI.text = "<color=red>YOU TRUSTED THE WRONG PERSON. They wait until you sleep... YOU DIED.</color>";
            // TODO: Trigger Game Over Screen or lethal night event here
        }
        else if (playerTrustedNPC && !currentNPC.isDangerous)
        {
            // Trusted a safe person -> WIN
            dialogueTextUI.text = "<color=green>You gained an ally. The night passes peacefully. YOU SURVIVED.</color>";
            // TODO: Trigger Next Day logic here
        }
        else if (!playerTrustedNPC && currentNPC.isDangerous)
        {
            // Turned away a dangerous person -> WIN
            dialogueTextUI.text = "<color=yellow>You locked them out. They bang on the door, but you are safe. YOU SURVIVED.</color>";
            // TODO: Trigger Next Day logic here
        }
        else if (!playerTrustedNPC && !currentNPC.isDangerous)
        {
            // Turned away a safe person -> SURVIVE, but maybe feel bad about it?
            dialogueTextUI.text = "<color=orange>You turned away an innocent person. They died in the forest. You survive, but at what cost?</color>";
            // TODO: Trigger Next Day logic here
        }
    }

    //private void EndDayPhase()
    //{
    //    isConversing = false;
    //    playerInputField.gameObject.SetActive(false);
    //    dialogueTextUI.text = "The day fades. The night begins...";
    //    conversationHistory = "";
    //}
}