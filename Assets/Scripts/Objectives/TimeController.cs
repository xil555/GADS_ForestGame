using System;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Clock (24h). Example: start 12 = noon; sunrise 4; sunset 18–19 for dusk by 7 PM")]
    [SerializeField]
    private float timeMultipler;


    [SerializeField]
    [Tooltip("In-game time when the scene starts (0–24). Use 12 for noon.")]
    private float startHour;

    [SerializeField]
    private TextMeshProUGUI timeText;

    private DateTime currentTime;

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

    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);

        lastCalendarDate = currentTime.Date;
    }

    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime + timeMultipler);

        while (currentTime.Date > lastCalendarDate)
        {
            lastCalendarDate = lastCalendarDate.AddDays(1);
            ObjectiveManager.Instance?.NotifyCalendarMidnightCrossed();
        }

        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void RotateSun()
    {
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
