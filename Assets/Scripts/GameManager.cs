using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("System References")]
    public OllamaManager ollama;

    [Header("UI References")]
    public TextMeshProUGUI dialogueTextUI;
    public Button trustButton;
    public Button turnAwayButton;

    private NPC currentNPC;

    void Start()
    {
        // hide buttons until the NPC actually speaks
        trustButton.gameObject.SetActive(false);
        turnAwayButton.gameObject.SetActive(false);
    }

    // call this method when the 1PM trigger happens and an NPC arrives
    public void OnNPCArrives(NPC arrivingNPC)
    {
        currentNPC = arrivingNPC;

        // show loading text while Ollama thinks
        dialogueTextUI.text = "The " + currentNPC.npcName + " approaches. They open their mouth to speak...\n\n<i>[Generating Response...]</i>";

        // sends the NPC's specific prompt to the local LLM
        ollama.GenerateDialogue(currentNPC.GetSystemPrompt(), DisplayDialogue);
    }

    private void DisplayDialogue(string generatedText)
    {
        // updates the UI with the text
        dialogueTextUI.text = generatedText;

        // let the player make their choice
        trustButton.gameObject.SetActive(true);
        turnAwayButton.gameObject.SetActive(true);
    }

    public void TrustNPC()
    {
        Debug.Log("Player trusted " + currentNPC.npcName);
        // add your logic to track this choice for the Night Phase survival mechanics
        EndDayPhase();
    }

    public void TurnAwayNPC()
    {
        Debug.Log("Player turned away " + currentNPC.npcName);
        // add your logic to track this choice for the Night Phase survival mechanics
        EndDayPhase();
    }

    private void EndDayPhase()
    {
        trustButton.gameObject.SetActive(false);
        turnAwayButton.gameObject.SetActive(false);
        dialogueTextUI.text = "The day fades. The night begins...";
        // triggers the transition to the night survival phase
    }
}