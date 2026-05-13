using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName;

    [Tooltip("The prompt sent to Ollama to generate this specific NPC's dialogue.")]
    [TextArea(3, 10)]
    public string systemPrompt;

    public string GetSystemPrompt()
    {
        return systemPrompt;
    }
}