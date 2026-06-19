using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives a hold-task progress bar. Survival interactables call
/// <see cref="SubmitHoldProgress"/> while the player is actively holding the task key;
/// this script applies the slider in <see cref="LateUpdate"/> so only one task wins per frame.
/// </summary>
[DefaultExecutionOrder(1001)]
public class TaskProgressUI : MonoBehaviour
{
    public static TaskProgressUI Instance { get; private set; }

    public const float HoldDurationSeconds = 5f;

    static bool s_hasProgress;
    static float s_normalizedProgress;

    [SerializeField] GameObject progressPanel;
    [SerializeField] Slider progressSlider;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HideProgressImmediate();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void LateUpdate()
    {
        if (s_hasProgress)
            ShowProgress(s_normalizedProgress);
        else
            HideProgressImmediate();

        s_hasProgress = false;
    }

    /// <summary>Report active hold progress for the current frame (0–1).</summary>
    public static void SubmitHoldProgress(float holdTimeSeconds, float durationSeconds = HoldDurationSeconds)
    {
        if (durationSeconds <= 0f)
            return;

        s_normalizedProgress = Mathf.Clamp01(holdTimeSeconds / durationSeconds);
        s_hasProgress = true;
    }

    public void ShowProgress(float normalizedProgress)
    {
        if (progressPanel != null)
            progressPanel.SetActive(true);

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = Mathf.Clamp01(normalizedProgress);
        }
    }

    public void HideProgress()
    {
        HideProgressImmediate();
    }

    void HideProgressImmediate()
    {
        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (progressSlider != null)
            progressSlider.value = 0f;
    }
}
