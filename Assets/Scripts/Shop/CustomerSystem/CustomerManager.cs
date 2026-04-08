using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomerModelSpawner))]
[RequireComponent(typeof(CustomerRouteMover))]
public class CustomerManager : MonoBehaviour
{
    private const float HundredPercent = 100f;

    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private float _firstCustomerDelay;
    [SerializeField] private CustomerSchedule _schedule;
    [SerializeField] private float _blinkDuration;
    [SerializeField] private ProductCountDisplay _productCountDisplay;

    private CustomerRouteMover _routeMover;
    private CustomerModelSpawner _spawner;

    private int _customerIndex;
    private bool _isDayStarting;
    private Coroutine _spawnCoroutine;
    private bool _isEndingDay;

    private List<CustomerPool> _scheduleList;

    public event Action<Customer> CustomerSpawned;
    public event Action DayStarted;
    public event Action CustomersEnded;
    public event Action DayEnded;

    public int CustomerIndex => _customerIndex;
    public Customer CurrentCustomer { get; private set; }
    public CustomerRouteMover CustomerRouteMover => _routeMover;

    private void Awake()
    {
        _spawner = GetComponent<CustomerModelSpawner>();
        _routeMover = GetComponent<CustomerRouteMover>();
        _productCountDisplay.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _routeMover.ReachedCounter += OnReachedCounter;
        _routeMover.LeftCafe += NextCustomer;
        _worldTime.DayFinished += OnDayFinished;
        CustomersEnded += OnCustomersEnded;
    }

    private void OnDisable()
    {
        _routeMover.ReachedCounter -= OnReachedCounter;
        _routeMover.LeftCafe -= NextCustomer;
        _worldTime.DayFinished -= OnDayFinished;
        CustomersEnded -= OnCustomersEnded;

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

        if (Darkness.Instance != null && Darkness.Instance.IsDark() == false)
        {
            Darkness.Instance.FadeIn(_blinkDuration / 2);
            yield return new WaitUntil(() => Darkness.Instance.IsDark());
        }

        if (Darkness.Instance != null && Darkness.Instance.IsDark())
        {
            Darkness.Instance.FadeOut(_blinkDuration / 2);
            yield return new WaitUntil(() => Darkness.Instance.IsDark() == false);
            yield return null;
        }

        StartNewDay();
    }

    public void EndDay()
    {
        if (_isEndingDay)
            return;

        _isEndingDay = true;

        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }

        if (_schedule.NextDaySchedule == null)
        {
            Debug.LogWarning($"[CustomerManager] {name}: No next day schedule assigned.");
            _isEndingDay = false;
            return;
        }
        _schedule = _schedule.NextDaySchedule;
        _scheduleList = _schedule.List;

        _isDayStarting = false;
        DayEnded?.Invoke();
        StartCoroutine(StartAfterFade());
        _isEndingDay = false;
    }

    public void StartNewDay()
    {
        if (_isDayStarting)
            return;

        StartCoroutine(NewDayRoutine());
        StartCoroutine(DayRoutine());
    }

    private IEnumerator NewDayRoutine()
    {
        _scheduleList = _schedule.List;

        _isDayStarting = true;
        DayStarted?.Invoke();

        while (FrostManager.Instance == null)
            yield return null;

        FrostManager.Instance.ResetAllWindows();
        FrostManager.Instance.SetWarm(SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "Day"));
    }

    private IEnumerator DayRoutine()
    {
        _customerIndex = 0;

        yield return new WaitForSecondsRealtime(_firstCustomerDelay);

        SpawnCustomer();
    }

    private void SpawnCustomer()
    {
        if (_scheduleList.Count == 0)
            return;

        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        _spawnCoroutine = StartCoroutine(SpawnCustomerCoroutine());
    }

    private IEnumerator SpawnCustomerCoroutine()
    {
        yield return null;

        _worldTime?.StartSmoothAddPercent(HundredPercent / _scheduleList.Count);

        GameObject prefab;

        try
        {
            prefab = _scheduleList[_customerIndex].GetCustomerFromPool();
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
        _productCountDisplay.gameObject.SetActive(true);
    }

    private void NextCustomer()
    {
        CurrentCustomer?.Despawn();
        CurrentCustomer = null;

        _customerIndex++;

        if (_customerIndex >= _scheduleList.Count)
        {
            CustomersEnded?.Invoke();
            return;
        }

        SpawnCustomer();
    }

    private void OnDayFinished() => EndDay();

    private void OnCustomersEnded() => EndDay();

    public void SetCurrentCustomer(int index) => _customerIndex = index;
}