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

    [Header("Hatch")]
    [SerializeField] private Hatch _hatch;
    [SerializeField, Range(0f,1f)] private float _openHatchCoef = 0.7f;

    [Header("Spawn")] 
    [SerializeField] private Transform _woodSpawnPoint;
    [SerializeField] private Wood _woodPrefab;
    [SerializeField] private int _maxWood = 4;

    private Queue<Wood> _woodQueue = new();
    private List<BurningWoodData> _burningWoods = new();
    private Coroutine _processQueueCoroutine;

    private float _hatchCoef = 1f;
    
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

    private void OnEnable()
    {
        if (_hatch != null)
            _hatch.StateChanged += OnHatchStateChanged;
    }

    private void OnDisable()
    {
        if (_hatch != null)
            _hatch.StateChanged -= OnHatchStateChanged;
    }

    private void OnHatchStateChanged(bool isOpen)
    {
        _hatchCoef = isOpen ? _openHatchCoef : 1f;

        UpdateTotalFirePower();
    }

    public void TryAddWood()
    {
        if (_woodQueue.Count + _burningWoods.Count >= _maxWood || (_hatch != null && !_hatch.IsOpen))
            return;

        Wood wood = Instantiate(_woodPrefab, _woodSpawnPoint.position, Random.rotationUniform);
        
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
                var wood = _woodQueue.Dequeue();
                var data = new BurningWoodData(wood);

                _burningWoods.Add(data);

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
        float total = 0f;

        foreach (var wood in _burningWoods)
            total += wood.CurrentPower;

        total *= _hatchCoef;

        FirePower = Mathf.Clamp(Mathf.RoundToInt(total), 0, MaxFirePower);

        FirePowerChanged?.Invoke(FirePower);
    }
    
    private IEnumerator BurnWood(BurningWoodData data)
    {
        var wood = data.Wood;

        float riseTime = _woodPower / _speedCoef;
        float peakTime = _woodFullPowerTime;
        float fadeTime = _woodPower / _fadeCoef;

        float totalTime = riseTime + peakTime + fadeTime;
        float t = 0;

        yield return null;

        while (t < totalTime && wood != null)
        {
            t += Time.deltaTime;

            float emission;
            float burn;
            float power;

            if (t < riseTime)
            {
                float k = t / riseTime;
                emission = k;
                burn = 0;
                power = _woodPower * k;
            }
            else if (t < riseTime + peakTime)
            {
                emission = 1;
                power = _woodPower;

                float pt = t - riseTime;
                float half = peakTime * 0.5f;

                burn = pt < half ? 0 : Mathf.Clamp01((pt - half) / half) * 0.5f;
            }
            else
            {
                float ft = (t - riseTime - peakTime) / fadeTime;

                emission = 1 - ft;
                power = _woodPower * (1 - ft);
                burn = 0.5f + ft * 0.5f;
            }

            data.CurrentPower = power;

            UpdateTotalFirePower();
            wood.SetVisualProgress(burn, emission, _hatch.OpenPercentage);

            yield return null;
        }

        if (wood != null)
        {
            _burningWoods.Remove(data);
            Destroy(wood.gameObject);
            UpdateTotalFirePower();
        }
    }
}