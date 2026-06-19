using UnityEngine;
using TMPro;


// Drives the Game Over canvas: sets failure copy, then activates the panel.
// Assign the panel root and a TextMeshProUGUI for the death reason.
public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    //Last reason shown; survives scene load so the Lose scene can display it.
    public static string LastFailureReason { get; private set; }

    static readonly string[] TrustedWrongPersonMessages =
    {
        "You let the wrong person in. The cabin was no longer safe.",
        "Mercy for a stranger became your last mistake.",
        "The door opened for them—and closed on everything you knew."
    };

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI failureReasonText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HideGameOverImmediate();
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(LastFailureReason))
            ShowGameOver(LastFailureReason);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public static string GetRandomTrustedWrongPersonMessage()
    {
        return TrustedWrongPersonMessages[Random.Range(0, TrustedWrongPersonMessages.Length)];
    }

    //Update failure text, then show the Game Over panel.</summary>
    public void ShowGameOver(string failureReason)
    {
        string message = string.IsNullOrWhiteSpace(failureReason)
            ? GetRandomTrustedWrongPersonMessage()
            : failureReason;

        LastFailureReason = message;

        if (failureReasonText != null)
            failureReasonText.text = message;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        HideGameOverImmediate();
    }

    void HideGameOverImmediate()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (failureReasonText != null)
            failureReasonText.text = string.Empty;
    }
}
