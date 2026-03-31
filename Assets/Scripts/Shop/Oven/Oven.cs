using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Oven : MonoBehaviour
{
    private const int MaxFirePower = 100;

    [SerializeField] private int _woodPower = 20;
    [SerializeField] private int _woodFullPowerTime = 20;
    [SerializeField] private float _speedCoef = 2f;
    [SerializeField] private float _fadeCoef = 2f;

    [Header("Spawn")] 
    [SerializeField] private Hatch _hatch;
    [SerializeField] private Transform _woodSpawnPoint;
    [SerializeField] private Wood _woodPrefab;
    [SerializeField] private int _maxWood = 4;

    private Queue<Wood> _woodQueue = new();
    private List<BurningWoodData> _burningWoods = new();
    private Coroutine _processQueueCoroutine;

    private bool _isFirstWood = true;

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

    public int FirePower { get; private set; }

    public void TryAddWood()
    {
        if (_woodQueue.Count + _burningWoods.Count >= _maxWood || (_hatch != null && !_hatch.IsOpen))
            return;

        Wood wood = Instantiate(_woodPrefab, _woodSpawnPoint.position, Random.rotation);

        wood.ResetWood();

        _woodQueue.Enqueue(wood);

        WoodAdded?.Invoke();

        if (_processQueueCoroutine == null)
            _processQueueCoroutine = StartCoroutine(ProcessWoodQueue());
    }

    private IEnumerator ProcessWoodQueue()
    {
        while (_woodQueue.Count > 0)
        {
            if (_burningWoods.Count < _maxWood)
            {
                Wood wood = _woodQueue.Dequeue();
                BurningWoodData data = new BurningWoodData(wood);

                _burningWoods.Add(data);

                if (_isFirstWood)
                {
                    _isFirstWood = false;
                    yield return new WaitForSeconds(0.1f);
                }

                StartCoroutine(StartBurnNextFrame(data));
            }

            yield return null;
        }

        _processQueueCoroutine = null;
    }

    private IEnumerator StartBurnNextFrame(BurningWoodData data)
    {
        yield return null;

        StartCoroutine(BurnWood(data));
    }

    private void UpdateTotalFirePower()
    {
        int totalPower = 0;

        foreach (var woodData in _burningWoods)
            totalPower += Mathf.RoundToInt(woodData.CurrentPower);

        FirePower = Mathf.Clamp(totalPower, 0, MaxFirePower);

        FirePowerChanged?.Invoke(FirePower);
    }

    private IEnumerator BurnWood(BurningWoodData woodData)
    {
        Wood wood = woodData.Wood;

        if (wood != null)
            wood.SetVisualProgress(0f, 0f);

        yield return null;

        float riseTime = _woodPower / _speedCoef;
        float peakTime = _woodFullPowerTime;
        float fadeTime = _woodPower / _fadeCoef;

        float totalTime = riseTime + peakTime + fadeTime;
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
            }
            else if (elapsedTime < riseTime + peakTime)
            {
                emissionProgress = 1f;
                currentPower = _woodPower;

                float peakElapsed = elapsedTime - riseTime;
                float burnStartTime = peakTime / 2f;

                if (peakElapsed < burnStartTime)
                {
                    burnProgress = 0f;
                }
                else
                {
                    float t = (peakElapsed - burnStartTime) / burnStartTime;
                    burnProgress = Mathf.Clamp01(t * 0.5f);
                }
            }
            else
            {
                float fadeElapsed = elapsedTime - (riseTime + peakTime);
                float t = fadeElapsed / fadeTime;

                emissionProgress = 1f - t;
                currentPower = _woodPower * (1f - t);

                burnProgress = Mathf.Clamp01(0.5f + t * 0.5f);
            }

            woodData.CurrentPower = currentPower;

            UpdateTotalFirePower();

            if (wood != null)
                wood.SetVisualProgress(burnProgress, emissionProgress);

            yield return null;
        }

        if (wood != null)
        {
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