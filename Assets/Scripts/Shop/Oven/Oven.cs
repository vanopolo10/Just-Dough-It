using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Oven : MonoBehaviour
{
    private const int MaxFirePower = 100;

    [SerializeField, Tooltip("Сколько силы дает одно бревно")] 
    private int _woodPower = 20;
    [SerializeField, Tooltip("Сколько длится горение бревна на своем пике")] 
    private int _woodFullPowerTime = 20;
    [SerializeField, Tooltip("Скорость разгорания бревна")]
    private float _speedCoef = 2f;
    [SerializeField, Tooltip("Скорость затухания бревна")] 
    private float _fadeCoef = 2f;

    [Header("Spawn")] 
    [SerializeField] private Hatch _hatch;
    [SerializeField] private Transform _woodSpawnPoint;
    [SerializeField] private Wood _woodPrefab;
    [SerializeField] private int _maxWood = 4;

    private Queue<Wood> _woodQueue = new();
    private List<BurningWoodData> _burningWoods = new();
    private Coroutine _processQueueCoroutine;
    
    private class BurningWoodData
    {
        public Wood Wood;
        public float CurrentPower;
        
        public BurningWoodData(Wood wood)
        {
            Wood = wood;
            CurrentPower = 0;
        }
    }
    
    public event Action WoodAdded;
    public event Action<int> FirePowerChanged;

    public int FirePower { get; private set; } = 0;
    
    public void TryAddWood()
    {
        if (_woodQueue.Count + _burningWoods.Count >= _maxWood || _hatch.IsOpen == false)
            return;

        Wood spawnedWood = Instantiate(_woodPrefab, _woodSpawnPoint.position, Random.rotation);
        _woodQueue.Enqueue(spawnedWood);
        
        WoodAdded?.Invoke();
        
        _processQueueCoroutine ??= StartCoroutine(ProcessWoodQueue());
    }
    
    private IEnumerator ProcessWoodQueue()
    {
        while (_woodQueue.Count > 0)
        {
            if (_burningWoods.Count < _maxWood)
            {
                Wood wood = _woodQueue.Dequeue();
                BurningWoodData woodData = new BurningWoodData(wood);
                _burningWoods.Add(woodData);
                StartCoroutine(BurnWood(woodData));
            }
            
            yield return null;
        }
        
        _processQueueCoroutine = null;
    }
    
    private void UpdateTotalFirePower()
    {
        int totalPower = 0;
        foreach (var woodData in _burningWoods)
        {
            totalPower += Mathf.RoundToInt(woodData.CurrentPower);
        }
        
        FirePower = Mathf.Clamp(totalPower, 0, MaxFirePower);
        FirePowerChanged?.Invoke(FirePower);
    }
    
    private IEnumerator BurnWood(BurningWoodData woodData)
    {
        Wood wood = woodData.Wood;
        
        float riseTime = _woodPower / _speedCoef;
        float peakStartTime = riseTime;
        float halfPeakTime = peakStartTime + (_woodFullPowerTime / 2f);
        float totalTime = riseTime + _woodFullPowerTime + (_woodPower / _fadeCoef);
        float elapsedTime = 0f;
        
        while (elapsedTime < totalTime && wood != null)
        {
            elapsedTime += Time.deltaTime;
            
            float emissionProgress;
            float burnProgress;
            float currentPower;
            
            if (elapsedTime < riseTime)
            {
                float t = elapsedTime / riseTime;
                emissionProgress = t;
                burnProgress = 0f;
                currentPower = _woodPower * t;
                
                woodData.CurrentPower = currentPower;
                UpdateTotalFirePower();
            }
            else if (elapsedTime < peakStartTime + _woodFullPowerTime)
            {
                emissionProgress = 1f;
                currentPower = _woodPower;
                
                if (elapsedTime < halfPeakTime)
                {
                    burnProgress = 0f;
                }
                else
                {
                    burnProgress = (elapsedTime - halfPeakTime) / (_woodFullPowerTime / 2f);
                    burnProgress = Mathf.Clamp01(burnProgress);
                }
                
                woodData.CurrentPower = currentPower;
                UpdateTotalFirePower();
            }
            else
            {
                float fadeElapsed = elapsedTime - (peakStartTime + _woodFullPowerTime);
                float fadeTotal = _woodPower / _fadeCoef;
                float fadeProgress = fadeElapsed / fadeTotal;
                
                emissionProgress = 1f - fadeProgress;
                burnProgress = 1f;
                currentPower = _woodPower * (1f - fadeProgress);
                
                woodData.CurrentPower = currentPower;
                UpdateTotalFirePower();
            }
            
            wood.SetVisualProgress(burnProgress, emissionProgress);
            
            yield return null;
        }
        
        if (wood != null)
        {
            wood.StopBurning();
            _burningWoods.Remove(woodData);
            Destroy(wood.gameObject);
            UpdateTotalFirePower();
        }
        
        if (_burningWoods.Count == 0 && _woodQueue.Count == 0)
        {
            FirePower = 0;
            FirePowerChanged?.Invoke(FirePower);
        }
    }
}