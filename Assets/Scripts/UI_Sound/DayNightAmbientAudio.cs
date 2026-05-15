using UnityEngine;

/// <summary>
/// Loops day vs night ambient beds from in-game clock hours (default: day 6:00–18:00, night 18:00–6:00).
/// Assign two looping <see cref="AudioClip"/>s and two <see cref="AudioSource"/>s (or leave one null to mute that half).
/// Reads time from <see cref="TimeController.CurrentTime"/>; runs after default script order so it matches the latest tick.
/// </summary>
[DefaultExecutionOrder(50)]
public class DayNightAmbientAudio : MonoBehaviour
{
    [SerializeField] TimeController timeController;

    [Tooltip("Inclusive start of “day” period (24h clock, e.g. 6 = 6 AM).")]
    [SerializeField] float dayPeriodStartHour = 6f;

    [Tooltip("Exclusive end of day / start of night (e.g. 18 = 6 PM). Must be greater than day start.")]
    [SerializeField] float nightPeriodStartHour = 18f;

    [SerializeField] AudioSource dayAmbientSource;
    [SerializeField] AudioSource nightAmbientSource;

    [SerializeField] AudioClip dayLoopClip;
    [SerializeField] AudioClip nightLoopClip;

    [Tooltip("Seconds to crossfade when switching day night.")]
    [SerializeField] float crossfadeDuration = 2f;

    bool _isDay;
    float _dayVol = 1f;
    float _nightVol;

    void Awake()
    {
        if (dayAmbientSource != null)
        {
            dayAmbientSource.loop = true;
            dayAmbientSource.playOnAwake = false;
        }

        if (nightAmbientSource != null)
        {
            nightAmbientSource.loop = true;
            nightAmbientSource.playOnAwake = false;
        }
    }

    void Start()
    {
        if (timeController == null)
        {
            var found = FindObjectsByType<TimeController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (found.Length > 0)
                timeController = found[0];
        }

        ApplyClipsIfNeeded();
        _isDay = IsDayPeriod();
        SnapVolumes(_isDay ? 1f : 0f, _isDay ? 0f : 1f);
        EnsurePlaying();
    }

    void Update()
    {
        if (timeController == null)
            return;

        ApplyClipsIfNeeded();

        bool day = IsDayPeriod();
        if (day != _isDay)
        {
            _isDay = day;
            EnsurePlaying();
        }

        float dayTarget = _isDay ? 1f : 0f;
        float nightTarget = _isDay ? 0f : 1f;
        float step = crossfadeDuration > 0.01f ? Time.deltaTime / crossfadeDuration : 1f;
        _dayVol = Mathf.MoveTowards(_dayVol, dayTarget, step);
        _nightVol = Mathf.MoveTowards(_nightVol, nightTarget, step);

        if (dayAmbientSource != null)
            dayAmbientSource.volume = _dayVol;
        if (nightAmbientSource != null)
            nightAmbientSource.volume = _nightVol;
    }

    bool IsDayPeriod()
    {
        float h = (float)timeController.CurrentTime.TimeOfDay.TotalHours;
        return h >= dayPeriodStartHour && h < nightPeriodStartHour;
    }

    void ApplyClipsIfNeeded()
    {
        if (dayAmbientSource != null && dayLoopClip != null && dayAmbientSource.clip != dayLoopClip)
        {
            bool was = dayAmbientSource.isPlaying;
            dayAmbientSource.clip = dayLoopClip;
            if (was)
                dayAmbientSource.Play();
        }

        if (nightAmbientSource != null && nightLoopClip != null && nightAmbientSource.clip != nightLoopClip)
        {
            bool was = nightAmbientSource.isPlaying;
            nightAmbientSource.clip = nightLoopClip;
            if (was)
                nightAmbientSource.Play();
        }
    }

    void SnapVolumes(float day, float night)
    {
        _dayVol = day;
        _nightVol = night;
        if (dayAmbientSource != null)
            dayAmbientSource.volume = day;
        if (nightAmbientSource != null)
            nightAmbientSource.volume = night;
    }

    void EnsurePlaying()
    {
        if (dayAmbientSource != null && dayLoopClip != null && !dayAmbientSource.isPlaying)
            dayAmbientSource.Play();
        if (nightAmbientSource != null && nightLoopClip != null && !nightAmbientSource.isPlaying)
            nightAmbientSource.Play();
    }
}
