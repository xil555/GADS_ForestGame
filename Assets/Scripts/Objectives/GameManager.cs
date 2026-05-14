using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("System References")]
    public OllamaManager ollama;
    public TimeController timeController;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextUI;
    public TMP_InputField playerInputField;
    public GameObject gameOverPanel; 
    public GameObject winPanel;      

    [Header("Interrogation Settings")]
    public int maxQuestions = 3;
    public KeyCode walkAwayKey = KeyCode.X;

    [Header("NPC Spawning System")]
    [Tooltip("Assign your 5 NPC prefabs here in order of days (Day 1 = index 0, Day 5 = index 4)")]
    public GameObject[] npcPrefabs;
    public Transform npcSpawnPoint;     
    public Transform cabinDoorWaypoint; 

    [Header("Game Rules")]
    [Tooltip("How many NPCs the player must beat to win the game.")]
    public int daysToWin = 3;

    [Header("Player Reference")]
    public FirstPersonController playerController;

    private NPC currentNPC;
    private GameObject activeNPC;
    private bool isConversing = false;
    private bool isShowingOutcome = false;
    private bool gameEnded = false;
    private int lastSpawnedDay = 0;

    public bool IsConversing() { return isConversing; }

    void Start()
    {
        dialoguePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        playerInputField.gameObject.SetActive(false);
        playerInputField.onSubmit.AddListener(OnPlayerAsksQuestion);
    }

    void Update()
    {
        if (gameEnded) return; 

        
        if (ObjectiveManager.Instance != null && timeController != null)
        {
            if (ObjectiveManager.Instance.CurrentDay != lastSpawnedDay && timeController.CurrentTime.TimeOfDay.TotalHours >= 13f)
            {
                lastSpawnedDay = ObjectiveManager.Instance.CurrentDay;
                SpawnDailyNPC(lastSpawnedDay);
            }
        }

        if (playerInputField.isFocused) return;

        if (isConversing)
        {
            
            if (Input.GetKeyDown(walkAwayKey))
            {
                ExitConversation();
                return;
            }

            if (!isShowingOutcome)
            {
                if (Input.GetKeyDown(KeyCode.Y)) TrustNPC();
                if (Input.GetKeyDown(KeyCode.N)) TurnAwayNPC();
            }
        }
    }

    private void SpawnDailyNPC(int day)
    {
        int index = day - 1; // Day 1 maps to Array Index 0

        // Check if we have a prefab for today
        if (index < npcPrefabs.Length && npcPrefabs[index] != null)
        {
            // Instantiate the archetype
            activeNPC = Instantiate(npcPrefabs[index], npcSpawnPoint.position, Quaternion.identity);
            NPC npcScript = activeNPC.GetComponent<NPC>();

            
            npcScript.cabinDoorWaypoint = cabinDoorWaypoint;
        }
    }

    public void StartConversation(NPC npc)
    {
        if (gameEnded) return;

        playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (timeController != null) timeController.isPaused = true;

        currentNPC = npc;
        isConversing = true;
        isShowingOutcome = false;

        dialoguePanel.SetActive(true);
        playerInputField.gameObject.SetActive(false);

        if (!currentNPC.hasStartedConversation)
        {
            currentNPC.hasStartedConversation = true;
            currentNPC.conversationHistory = currentNPC.GetSystemPrompt() +
                "\n\nThe player approaches the door. What is the very first thing you say to them? Keep it to one short sentence.";

            dialogueTextUI.text = "<i>[Generating initial response...]</i>";
            ollama.GenerateDialogue(currentNPC.conversationHistory, DisplayDialogue);
        }
        else
        {
            dialogueTextUI.text = $"<i>[You returned to {currentNPC.npcName}]</i>\n\nType your next question or make a choice.";
            playerInputField.gameObject.SetActive(true);
            playerInputField.text = "";
            playerInputField.ActivateInputField();
        }
    }

    public void OnPlayerAsksQuestion(string question)
    {
        if (string.IsNullOrWhiteSpace(question)) return;

        currentNPC.questionsAsked++;
        currentNPC.conversationHistory += "\nPlayer asks: " + question + "\nNPC responds (keep it to one short sentence):";

        dialogueTextUI.text = "<i>[NPC is thinking...]</i>";
        playerInputField.gameObject.SetActive(false);

        ollama.GenerateDialogue(currentNPC.conversationHistory, DisplayDialogue);
    }

    private void DisplayDialogue(string generatedText)
    {
        currentNPC.conversationHistory += " " + generatedText;

        if (currentNPC.questionsAsked < maxQuestions)
        {
            dialogueTextUI.text = $"<b>{currentNPC.npcName}:</b>\n\"{generatedText}\"" +
                $"\n\n<i>(Question {currentNPC.questionsAsked}/{maxQuestions} used. Type a question, press 'Y'/'N', or press '{walkAwayKey}' to walk away)</i>";

            playerInputField.gameObject.SetActive(true);
            playerInputField.text = "";
            playerInputField.ActivateInputField();
        }
        else
        {
            dialogueTextUI.text = $"<b>{currentNPC.npcName}:</b>\n\"{generatedText}\"" +
                $"\n\n<color=red><i>(Time is up! You MUST decide: Press 'Y' to Trust, 'N' to Turn Away)</i></color>";
            playerInputField.gameObject.SetActive(false);
        }
    }

    public void TrustNPC() { DetermineOutcome(true); }
    public void TurnAwayNPC() { DetermineOutcome(false); }

    private void DetermineOutcome(bool playerTrustedNPC)
    {
        isShowingOutcome = true;
        playerInputField.gameObject.SetActive(false);
        currentNPC.isDeadOrGone = true;

        bool isDead = false;
        string exitPrompt = $"\n\n<i>(Press '{walkAwayKey}' to close and return to your tasks)</i>";

        // THE SUDDEN DEATH LOGIC
        if (playerTrustedNPC && currentNPC.isDangerous)
        {
            dialogueTextUI.text = "<color=red>YOU TRUSTED THE WRONG PERSON... THEY KILL YOU.</color>";
            isDead = true;
        }
        else if (playerTrustedNPC && !currentNPC.isDangerous)
        {
            dialogueTextUI.text = "<color=green>You gained an ally. You survive the day.</color>" + exitPrompt;
        }
        else if (!playerTrustedNPC && currentNPC.isDangerous)
        {
            dialogueTextUI.text = "<color=yellow>You locked them out. They leave you alone. You survive.</color>" + exitPrompt;
        }
        else if (!playerTrustedNPC && !currentNPC.isDangerous)
        {
            dialogueTextUI.text = "<color=orange>You turned away an innocent person. You survive, but at what cost?</color>" + exitPrompt;
        }

        if (isDead)
        {
            TriggerGameOver();
        }
        else
        {
            // CHECK WIN CONDITION
            if (ObjectiveManager.Instance.CurrentDay >= daysToWin)
            {
                TriggerWin();
            }
            else
            {
                // If they survived today, the NPC vanishes to let them finish their tasks
                Destroy(activeNPC, 2f);
            }
        }
    }

    private void TriggerGameOver()
    {
        gameEnded = true;
        dialoguePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
    }

    private void TriggerWin()
    {
        gameEnded = true;
        dialoguePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(true);
    }

    private void ExitConversation()
    {
        isConversing = false;
        isShowingOutcome = false;

        dialoguePanel.SetActive(false);
        playerInputField.gameObject.SetActive(false);

        playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (timeController != null) timeController.isPaused = false;
    }
}