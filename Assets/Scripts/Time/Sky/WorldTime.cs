using System;
using System.Collections;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    private Coroutine _inGameTimeCoroutine;
    
    public GameTime InGameTime;
    
    public event Action<GameTime> TimeChanged;
    public event Action DayOver;
    
    private void Start()
    {
        InGameTime.Initialize();

        InGameTime.DayOver += OnDayOver;
        
        _inGameTimeCoroutine = StartCoroutine(TimeCoroutine());
    }
    
    private IEnumerator TimeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);

            if (InGameTime.Minutes == 59)
                InGameTime.SetTime(InGameTime.Hours + 1, 0);
            else
                InGameTime.SetTime(InGameTime.Hours, InGameTime.Minutes + 1);
        
            TimeChanged?.Invoke(InGameTime);
        }
    }

    private void OnDayOver()
    {
        StopCoroutine(_inGameTimeCoroutine);
        DayOver?.Invoke();
    }
    
    public struct GameTime
    {
        private const int MinHours = 8;
        private const int MaxHours = 18;

        public int Hours { get; private set; }
        public int Minutes { get; private set; }

        public float CompletePercent
        {
            get
            {
                int totalMinutesInDay = (MaxHours - MinHours) * 60;
                int passedMinutes = (Hours - MinHours) * 60 + Minutes;
                return Mathf.Clamp01((float)passedMinutes / totalMinutesInDay);
            }
        }

        public void Initialize()
        {
            SetTime(MinHours, 0);
        }

        public event Action DayOver;
    
        public void SetTime(int hours, int minutes)
        {
            Hours = Mathf.Clamp(hours, MinHours, MaxHours);
            Minutes = Mathf.Clamp(minutes, 0, 59);
            
            if (hours == MaxHours)
                DayOver?.Invoke();
        }
    }
}