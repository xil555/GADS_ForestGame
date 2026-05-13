
using UnityEngine;
using TMPro; // Assuming you are using TextMeshPro for the UI

public class DialogueManager : MonoBehaviour
{
    public OllamaManager ollama;
    public TextMeshProUGUI dialogueTextUI;

    public void TriggerNPCArrival()
    {
        dialogueTextUI.text = "Thinking..."; // let the player know it's loading

        // this is where we inject our specific game rules and NPC hidden nature
        string prompt = "You are a lost tourist separated from your group outside a forest cabin. " +
                        "You are secretly dangerous. " +
                        "The cabin owner asks: 'Who are you and what do you want?'. " +
                        "Reply in one short, slightly suspicious sentence.";

        // call the integration script and pass a method to handle the result
        ollama.GenerateDialogue(prompt, DisplayDialogue);
    }

    private void DisplayDialogue(string generatedText)
    {
        // this runs once Ollama finishes generating the text
        dialogueTextUI.text = generatedText;
    }
}