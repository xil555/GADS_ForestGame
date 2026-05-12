using UnityEngine;
using TMPro;

/// <summary>
/// Singleton that drives the center-screen interaction prompt. Assign a Canvas child panel
/// with a TextMeshProUGUI; the panel is toggled when showing or hiding prompts.
/// </summary>
public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance { get; private set; }

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
