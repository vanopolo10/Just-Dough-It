using System;
using System.Collections;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    private const float HundredPercent = 100f;
    
    [Header("Day Range")]
    [SerializeField] private int _minHours = 8;
    [SerializeField] private int _maxHours = 18;
    [SerializeField] private float _sunDuration = 20;

    public GameTime InGameTime { get; private set; }

    public event Action<GameTime> TimeChanged;
    public event Action DayOver;

    private bool _dayEnded;
    private Coroutine _smoothAddCoroutine;
    
    private void Awake()
    {
        InGameTime = new GameTime(_minHours, _maxHours);
        SetDayPercent(0f);
    }
    
    public void SetDayPercent(float percent)
    {
        if (_dayEnded)
            return;

        percent = Mathf.Clamp01(percent);

        InGameTime.SetPercent(percent);

        TimeChanged?.Invoke(InGameTime);

        if (percent >= 1f)
        {
            _dayEnded = true;
            DayOver?.Invoke();
        }
    }

    public void StartSmoothAddPercent(float percent)
    {
        if (_smoothAddCoroutine != null)
            StopCoroutine(_smoothAddCoroutine);
        
        _smoothAddCoroutine = StartCoroutine(SmoothAddPercent(percent / HundredPercent, _sunDuration));
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
    
    public void AddDayPercent(float deltaPercent)
    {
        Debug.Log("AddDayPercent called: " + deltaPercent);

        float normalized = deltaPercent / HundredPercent;
        SetDayPercent(InGameTime.CompletePercent + normalized);
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