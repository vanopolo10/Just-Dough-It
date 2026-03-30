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
    [SerializeField] private Transform _woodSpawnPoint;
    [SerializeField] private Wood _woodPrefab;
    [SerializeField] private int _maxWood = 4;

    private Queue<Wood> _woodQueue = new();
    private List<Wood> _burningWoods = new();
    private List<Coroutine> _burnCoroutines = new();
    
    public event Action WoodAdded;
    public event Action<int> FirePowerChanged;

    public int FirePower { get; private set; } = 0;
    
    public void TryAddWood()
    {
        if (_woodQueue.Count + _burningWoods.Count >= _maxWood)
        {
            Debug.Log($"Нельзя добавить бревно. Максимум: {_maxWood}, горит: {_burningWoods.Count}, в очереди: {_woodQueue.Count}");
            return;
        }

        Wood spawnedWood = Instantiate(_woodPrefab, _woodSpawnPoint.position, new Quaternion(Random.rotation.x, 0, Random.rotation.z, 1));
        _woodQueue.Enqueue(spawnedWood);
        
        WoodAdded?.Invoke();
        
        if (_burnCoroutines.Count == 0)
        {
            StartCoroutine(ProcessWoodQueue());
        }
    }
    
    private IEnumerator ProcessWoodQueue()
    {
        while (_woodQueue.Count > 0 || _burningWoods.Count > 0)
        {
            if (_woodQueue.Count > 0 && _burningWoods.Count < _maxWood)
            {
                Wood wood = _woodQueue.Dequeue();
                _burningWoods.Add(wood);
                Coroutine burnCoroutine = StartCoroutine(BurnWood(wood));
                _burnCoroutines.Add(burnCoroutine);
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private IEnumerator BurnWood(Wood wood)
    {
        float riseTime = _woodPower / _speedCoef;
        float burnProgress = 0f;
        
        while (burnProgress < 1f)
        {
            burnProgress += Time.deltaTime / riseTime;
            float progress = Mathf.Clamp01(burnProgress);
            
            if (wood != null)
                wood.SetBurnProgress(progress);
            
            FirePower = Mathf.Clamp(FirePower + 1, 0, MaxFirePower);
            FirePowerChanged?.Invoke(FirePower);
            
            yield return null;
        }
        
        if (wood != null)
            wood.SetBurnProgress(1f);
        
        yield return new WaitForSeconds(_woodFullPowerTime);
        
        float fadeTime = _woodPower / _fadeCoef;
        float fadeProgress = 0f;
        
        while (fadeProgress < 1f)
        {
            fadeProgress += Time.deltaTime / fadeTime;
            float progress = Mathf.Clamp01(1f - fadeProgress);
            
            if (wood != null)
                wood.SetBurnProgress(progress);
            
            FirePower = Mathf.Clamp(FirePower - 1, 0, MaxFirePower);
            FirePowerChanged?.Invoke(FirePower);
            
            yield return null;
        }
        
        if (wood != null)
        {
            wood.StopBurning();
            _burningWoods.Remove(wood);
            Destroy(wood.gameObject);
        }
        
        if (_burningWoods.Count == 0 && _woodQueue.Count == 0)
        {
            FirePower = 0;
            FirePowerChanged?.Invoke(FirePower);
        }
    }
}