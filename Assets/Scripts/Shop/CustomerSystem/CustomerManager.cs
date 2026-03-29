using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CustomerModelSpawner))]
[RequireComponent(typeof(CustomerRouteMover))]
public class CustomerManager : MonoBehaviour
{
    private const float HundredPercent = 100f;

    [SerializeField] private Button _nextDayButton;
    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private float _firstCustomerDelay;
    [SerializeField] private List<CustomerPool> _schedule;

    private CustomerRouteMover _routeMover;
    private CustomerModelSpawner _spawner;

    private int _currentIndex;
    private bool _isDayStarting;
    private Coroutine _spawnCoroutine;

    public event Action<Customer> CustomerSpawned;
    public event Action DayStarted;
    public event Action DayEnded;

    public Customer CurrentCustomer { get; private set; }

    private void Awake()
    {
        _spawner = GetComponent<CustomerModelSpawner>();
        _routeMover = GetComponent<CustomerRouteMover>();

        if (_nextDayButton != null)
            _nextDayButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _routeMover.ReachedCounter += OnReachedCounter;
        _routeMover.LeftCafe += NextCustomer;
    }

    private void OnDisable()
    {
        _routeMover.ReachedCounter -= OnReachedCounter;
        _routeMover.LeftCafe -= NextCustomer;

        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
    }

    private void Start()
    {
        StartCoroutine(StartAfterFade());
    }

    private IEnumerator StartAfterFade()
    {
        yield return null;

        if (Darkness.Instance != null && Darkness.Instance.IsDark())
        {
            Darkness.Instance.FadeOut();
            yield return new WaitUntil(() => Darkness.Instance.IsDark() == false);
            yield return null;
        }

        StartNewDay();
    }

    public void EndDay()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }

        _isDayStarting = false;
        Darkness.Instance.FadeIn();
    }

    public void StartNewDay()
    {
        if (_isDayStarting)
            return;

        _isDayStarting = true;
        StartCoroutine(DayRoutine());
    }

    private IEnumerator DayRoutine()
    {
        _currentIndex = 0;
        DayStarted?.Invoke();

        yield return new WaitForSecondsRealtime(_firstCustomerDelay);

        SpawnCustomer();
    }

    private void SpawnCustomer()
    {
        if (_schedule.Count == 0)
            return;

        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        _spawnCoroutine = StartCoroutine(SpawnCustomerCoroutine());
    }

    private IEnumerator SpawnCustomerCoroutine()
    {
        yield return null;

        _worldTime?.StartSmoothAddPercent(HundredPercent / _schedule.Count);

        GameObject prefab;

        try
        {
            prefab = _schedule[_currentIndex].GetCustomerFromPool();
        }
        catch
        {
            NextCustomer();
            yield break;
        }

        if (prefab == null)
        {
            NextCustomer();
            yield break;
        }

        GameObject spawned = _spawner.Spawn(prefab, null);

        if (!spawned)
        {
            NextCustomer();
            yield break;
        }

        CurrentCustomer = spawned.GetComponent<Customer>();

        if (!CurrentCustomer)
        {
            NextCustomer();
            yield break;
        }

        var animator = spawned.GetComponentInChildren<CustomerAnimatorController>();
        _routeMover.Initialize(animator);

        CustomerSpawned?.Invoke(CurrentCustomer);
        _spawnCoroutine = null;
    }

    private void OnReachedCounter()
    {
        CurrentCustomer?.OnReachedCounter();
    }

    private void NextCustomer()
    {
        CurrentCustomer?.Despawn();
        CurrentCustomer = null;

        _currentIndex++;

        if (_currentIndex >= _schedule.Count)
        {
            StartCoroutine(ShowNextDayButton());
            return;
        }

        SpawnCustomer();
    }

    private IEnumerator ShowNextDayButton()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        if (_nextDayButton != null)
            _nextDayButton.gameObject.SetActive(true);

        DayEnded?.Invoke();
    }
}