using System;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Clock (24h). Example: start 12 = noon; sunrise 4; sunset 18–19 for dusk by 7 PM")]
    [SerializeField]
    private float timeMultipler;

    [Tooltip("When true, time and sun do not advance (e.g. dialogue).")]
    public bool isPaused = false;

    [Header("Daily task deadline")]
    [Tooltip("In-game hour (24h clock) by which today's two objectives must be finished. If time reaches this with tasks incomplete, the clock freezes at this hour until they are done.")]
    [SerializeField]
    float dailyTasksDeadlineHour = 12f;

    [SerializeField]
    [Tooltip("In-game time when the scene starts (0–24). Use 12 for noon.")]
    private float startHour;

    [SerializeField]
    private TextMeshProUGUI timeText;

    DateTime currentTime;

    /// <summary>In-game calendar clock (driven in <see cref="Update"/>).</summary>
    public DateTime CurrentGameDateTime => currentTime;

    /// <summary>Same as <see cref="CurrentGameDateTime"/>.</summary>
    public DateTime CurrentTime => currentTime;

    [SerializeField]
    private Light sunLight;

    [SerializeField]
    [Tooltip("Time of day when night ends / sun arc begins (e.g. 4 = 4 AM).")]
    private float sunriseHour;

    [SerializeField]
    [Tooltip("Time when day ends / dusk–night arc begins (e.g. 18–19 for dark by 7 PM with your light curve).")]
    private float sunsetHour;

    [SerializeField]
    private Color dayAmbientLight;

    [SerializeField]
    private Color nightAmbientLight;

    [SerializeField]
    private AnimationCurve lightChangeCurve;

    [SerializeField]
    private Light moonLight;

    [SerializeField]
    private float maxSunLightIntensity;

    [SerializeField]
    private float maxMoonLightIntensity;

    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    DateTime lastCalendarDate;

    bool _deadlineHold;

    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);

        lastCalendarDate = currentTime.Date;
        ApplyNoonDeadlineClampIfNeeded();
    }

    void Update()
    {
        if (isPaused)
            return;

        if (_deadlineHold && AreDailyTasksComplete())
            _deadlineHold = false;

        if (!_deadlineHold)
            UpdateTimeOfDay();
        else
            RefreshClockText();

        RotateSun();
        UpdateLightSettings();
    }

    void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime + timeMultipler);

        while (currentTime.Date > lastCalendarDate)
        {
            lastCalendarDate = lastCalendarDate.AddDays(1);
            ObjectiveManager.Instance?.NotifyCalendarMidnightCrossed();
        }

        RefreshClockText();
        ApplyNoonDeadlineClampIfNeeded();
    }

    void RefreshClockText()
    {
        if (timeText != null)
            timeText.text = currentTime.ToString("HH:mm");
    }

    bool AreDailyTasksComplete()
    {
        var om = ObjectiveManager.Instance;
        if (om == null)
            return true;
        if (om.IsGameComplete)
            return true;
        return om.AreCurrentDaysObjectivesComplete();
    }

    void ApplyNoonDeadlineClampIfNeeded()
    {
        if (AreDailyTasksComplete())
            return;

        var om = ObjectiveManager.Instance;
        if (om == null || om.IsGameComplete)
            return;

        TimeSpan deadline = TimeSpan.FromHours(dailyTasksDeadlineHour);
        if (currentTime.TimeOfDay >= deadline)
        {
            currentTime = currentTime.Date + deadline;
            _deadlineHold = true;
        }
    }

    private void RotateSun()
    {
        if (sunLight == null)
            return;

        float sunLightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 100, (float)percentage);
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(100, 300, (float)percentage);
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private void UpdateLightSettings()
    {
        if (sunLight == null || moonLight == null)
            return;

        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan diff = toTime - fromTime;
        if (diff.TotalSeconds < 0)
            diff += TimeSpan.FromHours(24);
        return diff;
    }
}
