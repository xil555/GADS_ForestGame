using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName;

    [TextArea(3, 10)]
    public string systemPrompt;

    [Header("Hidden Nature")]
    [Tooltip("Check this box if the NPC is secretly dangerous.")]
    public bool isDangerous;

    public string GetSystemPrompt()
    {
        return systemPrompt;
    }
}