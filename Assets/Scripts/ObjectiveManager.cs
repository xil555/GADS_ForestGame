using System.Collections.Generic;
using UnityEngine;
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

    [Header("Delivery visuals near house (start disabled in scene)")]
    [SerializeField] GameObject logCrateWithLogs;
    [SerializeField] GameObject mushroomCrateOrPile;
    [SerializeField] GameObject waterBarrels;

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
            SetLogCrateVisible(true);

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
            SetMushroomDeliveryVisible(true);

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
    }

    public void CompleteFence()
    {
        if (IsGameComplete)
            return;

        if (!HasObjective(ObjectiveType.Fence) || fenceComplete)
            return;

        fenceComplete = true;
        UpdateUI();
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
        if (chopTreesTMP != null)
        {
            chopTreesTMP.text = treesChopped >= TreesRequired
                ? "Chop Trees: Completed"
                : $"Chop Trees: {treesChopped}/{TreesRequired}";
        }

        if (mushroomTMP != null)
        {
            mushroomTMP.text = mushroomsCollected >= MushroomsRequired
                ? "Collect Mushrooms: Completed"
                : $"Collect Mushrooms: {mushroomsCollected}/{MushroomsRequired}";
        }

        if (fireTMP != null)
            fireTMP.text = fireComplete ? "Start Fire: Completed" : "Start Fire: Incomplete";

        if (waterTMP != null)
            waterTMP.text = waterComplete ? "Collect Water: Completed" : "Collect Water: Incomplete";

        if (fenceTMP != null)
            fenceTMP.text = fenceComplete ? "Repair Fence: Completed" : "Repair Fence: Incomplete";
    }
}
