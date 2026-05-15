using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Five-day run: day 1–5 each get one resource task (Logs or Mushroom) and one home task (Fire, Water, or Fence).
/// Logs/Mushroom are never both active; Fire/Fence/Water only one survival slot—never two from the same survival pool.
/// <see cref="TimeController"/> calls <see cref="NotifyCalendarMidnightCrossed"/> at midnight to advance the day and re-roll.
/// After the calendar rolls past day 5, the run is marked complete and objectives stop progressing.
/// </summary>
public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    public const int MaxDays = 5;

    const int TreesRequired = 5;
    const int MushroomsRequired = 10;

    [Header("Day UI (top-left, e.g. \"Day 1\")")]
    [SerializeField] TextMeshProUGUI dayLabelTMP;

    [Header("Daily Tasks UI — parent GameObjects (one per line)")]
    [SerializeField] GameObject chopTreesText;
    [SerializeField] GameObject mushroomText;
    [SerializeField] GameObject fireText;
    [SerializeField] GameObject waterText;
    [SerializeField] GameObject fenceText;

    [Header("Daily Tasks UI — TextMeshPro (separate objects)")]
    [SerializeField] TextMeshProUGUI chopTreesTMP;
    [SerializeField] TextMeshProUGUI mushroomTMP;
    [SerializeField] TextMeshProUGUI fireTMP;
    [SerializeField] TextMeshProUGUI waterTMP;
    [SerializeField] TextMeshProUGUI fenceTMP;

    [Header("Daily Tasks UI — icons (optional; place beside TMP in layout)")]
    [SerializeField] Image chopTreesIcon;
    [SerializeField] Image mushroomIcon;
    [SerializeField] Image fireIcon;
    [SerializeField] Image waterIcon;
    [SerializeField] Image fenceIcon;

    [Header("Survival row icon sprites (optional; swaps with Completed / Incomplete)")]
    [SerializeField] Sprite fireIncompleteSprite;
    [SerializeField] Sprite fireCompleteSprite;
    [SerializeField] Sprite waterIncompleteSprite;
    [SerializeField] Sprite waterCompleteSprite;
    [SerializeField] Sprite fenceIncompleteSprite;
    [SerializeField] Sprite fenceCompleteSprite;

    [Header("Delivery visuals near house (start disabled in scene)")]
    [SerializeField] GameObject logCrateWithLogs;
    [SerializeField] GameObject mushroomCrateOrPile;
    [SerializeField] GameObject waterBarrels;

    [Header("Daily Tasks panel root (optional)")]
    [Tooltip("UI root only — do not parent world mushrooms or fence objects under this. Hidden when today's two tasks are complete; shown again on the next day.")]
    [SerializeField] GameObject dailyTasksPanelRoot;

    [Header("Task completion SFX (optional)")]
    [Tooltip("If null, one-shots play at the main camera position.")]
    [SerializeField] AudioSource taskSfxSource;
    [SerializeField] AudioClip logsTaskCompleteClip;
    [SerializeField] AudioClip mushroomTaskCompleteClip;
    [SerializeField] AudioClip fireTaskCompleteClip;
    [SerializeField] AudioClip waterTaskCompleteClip;
    [SerializeField] AudioClip fenceTaskCompleteClip;

    ObjectiveType resourceObjective;
    ObjectiveType survivalObjective;

    int treesChopped;
    int mushroomsCollected;

    bool fireComplete;
    bool waterComplete;
    bool fenceComplete;

    public int CurrentDay { get; private set; } = 1;
    public bool IsGameComplete { get; private set; }

    readonly List<TreeChop> treeCache = new List<TreeChop>();
    readonly List<MushroomCollect> mushroomCache = new List<MushroomCollect>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CacheWorldInteractables();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        CurrentDay = 1;
        IsGameComplete = false;
        SetDeliveryVisualsActive(false);
        RollObjectivesForCurrentDay();
        ResetTreesAndMushroomsInWorld();
        ResetSurvivalInteractables();
        DisableAllTaskRows();
        EnableCorrectUI();
        UpdateUI();
        UpdateDayLabel();
    }

    /// <summary>Called from <see cref="TimeController"/> when the in-game date crosses midnight.</summary>
    public void NotifyCalendarMidnightCrossed()
    {
        if (IsGameComplete)
            return;

        if (CurrentDay >= MaxDays)
        {
            IsGameComplete = true;
            UpdateDayLabel();
            RefreshDailyTasksPanelVisibility();
            return;
        }

        CurrentDay++;
        BeginNewDay();
        UpdateDayLabel();
    }

    [ContextMenu("Begin New Day (roll objectives)")]
    void DebugBeginNewDay()
    {
        if (IsGameComplete)
            return;

        BeginNewDay();
        UpdateDayLabel();
    }

    void CacheWorldInteractables()
    {
        treeCache.Clear();
        mushroomCache.Clear();
        treeCache.AddRange(FindObjectsByType<TreeChop>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        mushroomCache.AddRange(FindObjectsByType<MushroomCollect>(FindObjectsInactive.Include, FindObjectsSortMode.None));
    }

    /// <summary>Roll new objectives, reset progress, re-enable chopped trees / picked mushrooms, hide delivery props.</summary>
    public void BeginNewDay()
    {
        if (IsGameComplete)
            return;

        treesChopped = 0;
        mushroomsCollected = 0;
        fireComplete = false;
        waterComplete = false;
        fenceComplete = false;

        SetDeliveryVisualsActive(false);

        RollObjectivesForCurrentDay();

        ResetTreesAndMushroomsInWorld();
        ResetSurvivalInteractables();

        DisableAllTaskRows();
        EnableCorrectUI();
        UpdateUI();
    }

    /// <summary>
    /// Group A (resource): exactly one of Logs or Mushroom per day—never both.
    /// Group B (home / survival): exactly one of Fire, Water, or Fence—Fire and Fence never share the same day as two tasks.
    /// </summary>
    void RollObjectivesForCurrentDay()
    {
        int resourceRandom = Random.Range(0, 2);
        resourceObjective = resourceRandom == 0 ? ObjectiveType.Logs : ObjectiveType.Mushroom;

        int survivalRandom = Random.Range(0, 3);
        if (survivalRandom == 0)
            survivalObjective = ObjectiveType.Fire;
        else if (survivalRandom == 1)
            survivalObjective = ObjectiveType.Water;
        else
            survivalObjective = ObjectiveType.Fence;
    }

    void UpdateDayLabel()
    {
        if (dayLabelTMP == null)
            return;

        if (IsGameComplete)
        {
            dayLabelTMP.text = "Day 5 — complete";
            return;
        }

        dayLabelTMP.text = $"Day {CurrentDay}";
    }

    void ResetTreesAndMushroomsInWorld()
    {
        if (treeCache.Count == 0 && mushroomCache.Count == 0)
            CacheWorldInteractables();

        for (int i = 0; i < treeCache.Count; i++)
        {
            if (treeCache[i] != null)
                treeCache[i].ResetForNewDay();
        }

        for (int i = 0; i < mushroomCache.Count; i++)
        {
            if (mushroomCache[i] != null)
                mushroomCache[i].ResetForNewDay();
        }
    }

    void ResetSurvivalInteractables()
    {
        foreach (var f in FindObjectsByType<FireInteract>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            f.ResetForNewDay();

        foreach (var w in FindObjectsByType<WaterCollect>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            w.ResetForNewDay();

        foreach (var fr in FindObjectsByType<FenceRepair>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            fr.ResetForNewDay();
    }

    void DisableAllTaskRows()
    {
        if (chopTreesText != null) chopTreesText.SetActive(false);
        if (mushroomText != null) mushroomText.SetActive(false);
        if (fireText != null) fireText.SetActive(false);
        if (waterText != null) waterText.SetActive(false);
        if (fenceText != null) fenceText.SetActive(false);
    }

    void EnableCorrectUI()
    {
        if (resourceObjective == ObjectiveType.Logs && chopTreesText != null)
            chopTreesText.SetActive(true);

        if (resourceObjective == ObjectiveType.Mushroom && mushroomText != null)
            mushroomText.SetActive(true);

        if (survivalObjective == ObjectiveType.Fire && fireText != null)
            fireText.SetActive(true);

        if (survivalObjective == ObjectiveType.Water && waterText != null)
            waterText.SetActive(true);

        if (survivalObjective == ObjectiveType.Fence && fenceText != null)
            fenceText.SetActive(true);
    }

    public bool HasObjective(ObjectiveType type)
    {
        if (IsGameComplete)
            return false;

        return resourceObjective == type || survivalObjective == type;
    }

    /// <summary>False when the mushroom quota is reached for the day (or it is not a mushroom day).</summary>
    public bool CanCollectMoreMushrooms()
    {
        return HasObjective(ObjectiveType.Mushroom) && mushroomsCollected < MushroomsRequired;
    }

    /// <summary>False once the fence survival task has been completed for the day.</summary>
    public bool CanProgressFenceRepair()
    {
        return HasObjective(ObjectiveType.Fence) && !fenceComplete;
    }

    public bool TryChopTree()
    {
        if (IsGameComplete)
            return false;

        if (!HasObjective(ObjectiveType.Logs))
            return false;

        if (treesChopped >= TreesRequired)
            return false;

        treesChopped++;
        UpdateUI();
        if (treesChopped >= TreesRequired)
        {
            SetLogCrateVisible(true);
            PlayTaskSfx(logsTaskCompleteClip);
        }

        return true;
    }

    public bool TryCollectMushroom()
    {
        if (IsGameComplete)
            return false;

        if (!HasObjective(ObjectiveType.Mushroom))
            return false;

        if (mushroomsCollected >= MushroomsRequired)
            return false;

        mushroomsCollected++;
        UpdateUI();
        if (mushroomsCollected >= MushroomsRequired)
        {
            SetMushroomDeliveryVisible(true);
            PlayTaskSfx(mushroomTaskCompleteClip);
        }

        return true;
    }

    public void CompleteFire()
    {
        if (IsGameComplete)
            return;

        if (!HasObjective(ObjectiveType.Fire) || fireComplete)
            return;

        fireComplete = true;
        UpdateUI();
        PlayTaskSfx(fireTaskCompleteClip);
    }

    public void CompleteWater()
    {
        if (IsGameComplete)
            return;

        if (!HasObjective(ObjectiveType.Water) || waterComplete)
            return;

        waterComplete = true;
        UpdateUI();
        SetWaterBarrelsVisible(true);
        PlayTaskSfx(waterTaskCompleteClip);
    }

    public void CompleteFence()
    {
        if (IsGameComplete)
            return;

        if (!HasObjective(ObjectiveType.Fence) || fenceComplete)
            return;

        fenceComplete = true;
        UpdateUI();
        PlayTaskSfx(fenceTaskCompleteClip);
    }

    void PlayTaskSfx(AudioClip clip)
    {
        if (clip == null)
            return;

        if (taskSfxSource != null)
            taskSfxSource.PlayOneShot(clip);
        else
        {
            Vector3 pos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
            AudioSource.PlayClipAtPoint(clip, pos);
        }
    }

    void SetDeliveryVisualsActive(bool active)
    {
        if (logCrateWithLogs != null) logCrateWithLogs.SetActive(active);
        if (mushroomCrateOrPile != null) mushroomCrateOrPile.SetActive(active);
        if (waterBarrels != null) waterBarrels.SetActive(active);
    }

    void SetLogCrateVisible(bool visible)
    {
        if (logCrateWithLogs != null)
            logCrateWithLogs.SetActive(visible);
    }

    void SetMushroomDeliveryVisible(bool visible)
    {
        if (mushroomCrateOrPile != null)
            mushroomCrateOrPile.SetActive(visible);
    }

    void SetWaterBarrelsVisible(bool visible)
    {
        if (waterBarrels != null)
            waterBarrels.SetActive(visible);
    }

    void UpdateUI()
    {
        bool treesUseCompact = chopTreesIcon != null;
        if (chopTreesTMP != null)
        {
            chopTreesTMP.text = treesUseCompact
                ? $"{treesChopped} / {TreesRequired}"
                : (treesChopped >= TreesRequired
                    ? "Chop Trees: Completed"
                    : $"Chop Trees: {treesChopped}/{TreesRequired}");
        }

        bool mushroomsUseCompact = mushroomIcon != null;
        if (mushroomTMP != null)
        {
            mushroomTMP.text = mushroomsUseCompact
                ? $"{mushroomsCollected} / {MushroomsRequired}"
                : (mushroomsCollected >= MushroomsRequired
                    ? "Collect Mushrooms: Completed"
                    : $"Collect Mushrooms: {mushroomsCollected}/{MushroomsRequired}");
        }

        if (fireTMP != null)
        {
            fireTMP.text = fireIcon != null
                ? (fireComplete ? "Completed" : "Incomplete")
                : (fireComplete ? "Start Fire: Completed" : "Start Fire: Incomplete");
        }

        if (waterTMP != null)
        {
            waterTMP.text = waterIcon != null
                ? (waterComplete ? "Completed" : "Incomplete")
                : (waterComplete ? "Collect Water: Completed" : "Collect Water: Incomplete");
        }

        if (fenceTMP != null)
        {
            fenceTMP.text = fenceIcon != null
                ? (fenceComplete ? "Completed" : "Incomplete")
                : (fenceComplete ? "Repair Fence: Completed" : "Repair Fence: Incomplete");
        }

        ApplySurvivalRowSprite(fireIcon, fireComplete, fireCompleteSprite, fireIncompleteSprite);
        ApplySurvivalRowSprite(waterIcon, waterComplete, waterCompleteSprite, waterIncompleteSprite);
        ApplySurvivalRowSprite(fenceIcon, fenceComplete, fenceCompleteSprite, fenceIncompleteSprite);

        RefreshDailyTasksPanelVisibility();
    }

    void RefreshDailyTasksPanelVisibility()
    {
        if (dailyTasksPanelRoot == null)
            return;

        bool hide = IsGameComplete || AreCurrentDaysObjectivesComplete();
        dailyTasksPanelRoot.SetActive(!hide);
    }

    public bool AreCurrentDaysObjectivesComplete()
    {
        bool resourceOk = resourceObjective == ObjectiveType.Logs
            ? treesChopped >= TreesRequired
            : mushroomsCollected >= MushroomsRequired;

        bool survivalOk = survivalObjective switch
        {
            ObjectiveType.Fire => fireComplete,
            ObjectiveType.Water => waterComplete,
            ObjectiveType.Fence => fenceComplete,
            _ => false
        };

        return resourceOk && survivalOk;
    }

    static void ApplySurvivalRowSprite(Image icon, bool complete, Sprite whenComplete, Sprite whenIncomplete)
    {
        if (icon == null)
            return;

        Sprite chosen = complete ? whenComplete : whenIncomplete;
        if (chosen != null)
            icon.sprite = chosen;
    }
}
