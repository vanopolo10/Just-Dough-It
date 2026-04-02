using System;
using System.Collections;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    private const float HundredPercent = 100f;

    [SerializeField] private bool _isTutorial;
    [SerializeField][Range(0, HundredPercent)] private float _tutorialTime;

    [SerializeField] private CustomerManager _customerManager;

    [SerializeField] private int _minHours = 8;
    [SerializeField] private int _maxHours = 18;
    [SerializeField] private float _secondsPerPercent = 0.2f;
    
    private bool _pendingPreferSunrise;
    private bool _hasPendingChange;

    private bool _dayEnded;
    private Coroutine _smoothAddCoroutine;
    
    public event Action<GameTime> TimeChanged;

    public GameTime InGameTime { get; private set; }
    public bool PreferSunrise { get; private set; }

    private void Awake()
    {
        PreferSunrise = SaveSystem.LoadData<bool>(SaveSystem.SelectedSave, "DoPreferSunrises");
        OnDayStarted();
    }

    private void OnEnable()
    {
        _customerManager.DayStarted += OnDayStarted;
    }

    private void OnDisable()
    {
        _customerManager.DayStarted -= OnDayStarted;
    }

    private void Start()
    {
        SetDayPercent(_isTutorial ? _tutorialTime : 0f);
    }

    public void SetPreference(bool preferSunrise)
    {
        _pendingPreferSunrise = preferSunrise;
        _hasPendingChange = true;
    }

    private void ApplyPendingPreference()
    {
        if (!_hasPendingChange)
            return;

        PreferSunrise = _pendingPreferSunrise;
        SaveSystem.SaveData(SaveSystem.SelectedSave, "DoPreferSunrises", PreferSunrise);
        _hasPendingChange = false;
    }

    public void StartSmoothAddPercent(float percent)
    {
        if (_isTutorial)
            return;

        if (_smoothAddCoroutine != null)
            StopCoroutine(_smoothAddCoroutine);

        float duration = percent * _secondsPerPercent;
        _smoothAddCoroutine = StartCoroutine(SmoothAddPercent(percent / HundredPercent, duration));
    }

    private void OnDayStarted()
    {
        ApplyPendingPreference();

        _dayEnded = false;
        InGameTime = new GameTime(_minHours, _maxHours);
        SetDayPercent(0f);
    }

    private void SetDayPercent(float percent)
    {
        if (_dayEnded)
            return;

        percent = Mathf.Clamp01(percent);

        InGameTime.SetPercent(percent);

        TimeChanged?.Invoke(InGameTime);

        if (percent >= 1f)
        {
            _dayEnded = true;
        }
    }

    private IEnumerator SmoothAddPercent(float percentToAdd, float duration)
    {
        float start = InGameTime.CompletePercent;
        float target = Mathf.Clamp01(start + percentToAdd);

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Lerp(start, target, t / duration);
            SetDayPercent(p);
            yield return null;
        }

        SetDayPercent(target);
    }

    public class GameTime
    {
        private const int ClockPower = 60;

        private readonly int _minHours;
        private readonly int _maxHours;

        public float CompletePercent { get; private set; }
        public int Hours { get; private set; }
        public int Minutes { get; private set; }

        public GameTime(int minHours, int maxHours)
        {
            _minHours = minHours;
            _maxHours = maxHours;
        }

        public void SetPercent(float percent)
        {
            CompletePercent = Mathf.Clamp01(percent);

            float totalMinutes = (_maxHours - _minHours) * ClockPower;
            float passedMinutes = totalMinutes * CompletePercent;

            int total = Mathf.FloorToInt(passedMinutes);

            Hours = _minHours + total / ClockPower;
            Minutes = total % ClockPower;
        }
    }
}