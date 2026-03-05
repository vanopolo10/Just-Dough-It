using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CustomerModelSpawner))]
[RequireComponent(typeof(CustomerRouteMover))]
public class CustomerManager : MonoBehaviour
{
    private const float HundredPercent = 100f;

    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private List<CustomerPool> _schedule;

    private CustomerRouteMover _routeMover;
    private CustomerModelSpawner _spawner;

    private int _currentIndex;
    private Customer _currentCustomer;

    public UnityEvent DayStarted = new();
    public UnityEvent DayEnded = new();
    public UnityEvent CustomerSpawned = new();
    
    public Customer CurrentCustomer => _currentCustomer;

    private void Awake()
    {
        _spawner = GetComponent<CustomerModelSpawner>();
        _routeMover = GetComponent<CustomerRouteMover>();
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
    }

    private void Start()
    {
        StartNewDay();
    }

    public void StartNewDay()
    {
        _currentIndex = 0;
        DayStarted.Invoke();
        SpawnCustomer();
    }

    private void SpawnCustomer()
    {
        if (_schedule.Count == 0)
        {
            Debug.LogError("Schedule is empty.");
            return;
        }

        _worldTime?.StartSmoothAddPercent(HundredPercent / _schedule.Count);

        GameObject prefab = _schedule[_currentIndex].GetCustomerFromPool();
        GameObject spawned = _spawner.Spawn(prefab, null);

        if (!spawned)
        {
            Debug.LogError("Customer spawn failed.");
            return;
        }

        _currentCustomer = spawned.GetComponent<Customer>();
        if (!_currentCustomer)
        {
            Debug.LogError("Customer component missing.");
            return;
        }

        var animator = spawned.GetComponentInChildren<CustomerAnimatorController>();
        _routeMover.Initialize(animator);

        CustomerSpawned.Invoke();
    }

    private void OnReachedCounter()
    {
        _currentCustomer?.OnReachedCounter();
    }

    private void NextCustomer()
    {
        _currentCustomer?.Despawn();

        _currentIndex++;
        if (_currentIndex >= _schedule.Count)
        {
            EndDay();
            return;
        }

        SpawnCustomer();
    }

    private void EndDay()
    {
        DayEnded.Invoke();
        Invoke(nameof(StartNewDay), 10f);
    }
}