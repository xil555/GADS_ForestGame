using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Lives on the Lose scene. <see cref="GameManager"/> calls <see cref="QueueFailureReason"/>
/// before loading that scene; this component shows the queued message on start.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    /// <summary>Failure copy passed from the game scene before the Lose scene loads.</summary>
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
    }

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!string.IsNullOrEmpty(LastFailureReason))
            ShowGameOver(LastFailureReason);
        else
            HideGameOverImmediate();
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

    /// <summary>Call from the game scene before loading the Lose scene.</summary>
    public static void QueueFailureReason(string failureReason)
    {
        LastFailureReason = ResolveMessage(failureReason);
    }

    /// <summary>Update failure text, then show the Game Over panel in this scene.</summary>
    public void ShowGameOver(string failureReason)
    {
        LastFailureReason = ResolveMessage(failureReason);

        if (failureReasonText != null)
            failureReasonText.text = LastFailureReason;

        if (gameOverPanel != null)
        {
            SetPanelRaycastsForDisplayOnly(gameOverPanel);
            gameOverPanel.SetActive(true);
        }
    }

    public void HideGameOver()
    {
        HideGameOverImmediate();
    }

    static string ResolveMessage(string failureReason)
    {
        return string.IsNullOrWhiteSpace(failureReason)
            ? GetRandomTrustedWrongPersonMessage()
            : failureReason;
    }

    void HideGameOverImmediate()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (failureReasonText != null)
            failureReasonText.text = string.Empty;
    }

    /// <summary>Keep failure copy visible without blocking Main Menu / Quit buttons underneath.</summary>
    static void SetPanelRaycastsForDisplayOnly(GameObject panel)
    {
        foreach (var graphic in panel.GetComponentsInChildren<Graphic>(true))
        {
            if (graphic.GetComponentInParent<Button>() != null)
                continue;

            graphic.raycastTarget = false;
        }
    }
}
