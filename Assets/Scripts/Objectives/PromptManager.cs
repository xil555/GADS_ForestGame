using UnityEngine;
using TMPro;

/// <summary>
/// Singleton that drives the center-screen interaction prompt. Assign a Canvas child panel
/// with a TextMeshProUGUI; the panel is toggled when showing or hiding prompts.
/// Interactables call <see cref="SubmitPromptCandidate"/> from <see cref="MonoBehaviour.LateUpdate"/>;
/// this script runs later (execution order) and applies the highest-priority candidate for the frame.
/// </summary>
[DefaultExecutionOrder(1000)]
public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance { get; private set; }

    const int PriorityFenceWaterFire = 60;
    const int PriorityMushroom = 50;
    const int PriorityTreeChop = 40;

    static string s_pendingMessage;
    static int s_pendingPriority = int.MinValue;
    static bool s_hasCandidate;

    [SerializeField] GameObject promptPanel;
    [SerializeField] TextMeshProUGUI promptText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HidePromptImmediate();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void LateUpdate()
    {
        if (s_hasCandidate)
            ShowPrompt(s_pendingMessage);
        else
            HidePromptImmediate();

        s_hasCandidate = false;
        s_pendingPriority = int.MinValue;
    }

    /// <summary>Call from <see cref="MonoBehaviour.LateUpdate"/>; highest priority wins when several overlap.</summary>
    public static void SubmitPromptCandidate(string message, int priority)
    {
        if (string.IsNullOrEmpty(message))
            return;

        if (!s_hasCandidate || priority >= s_pendingPriority)
        {
            s_pendingMessage = message;
            s_pendingPriority = priority;
            s_hasCandidate = true;
        }
    }

    public static int PrioritySurvivalHold => PriorityFenceWaterFire;
    public static int PriorityMushroomCollect => PriorityMushroom;
    public static int PriorityTreeChopRay => PriorityTreeChop;

    public void ShowPrompt(string message)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = message;
    }

    public void HidePrompt()
    {
        HidePromptImmediate();
    }

    void HidePromptImmediate()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);

        if (promptText != null)
            promptText.text = string.Empty;
    }
}
