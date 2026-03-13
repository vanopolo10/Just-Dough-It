using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CustomerModelSpawner))]
[RequireComponent(typeof(CustomerRouteMover))]
public class CustomerManager : MonoBehaviour
{
    private const float HundredPercent = 100f;

    [SerializeField] private Button _nextDayButton;
    [SerializeField] private Darkness _darkness;
    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private float _firstCustomerDelay;
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
        _nextDayButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _routeMover.ReachedCounter += OnReachedCounter;
        _routeMover.LeftCafe += NextCustomer;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        _routeMover.ReachedCounter -= OnReachedCounter;
        _routeMover.LeftCafe -= NextCustomer;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartNewDay();
    }

    public void StartNewDay()
    {
        _darkness.WakeUp();
        _currentIndex = 0;
        DayStarted.Invoke();
        Invoke(nameof(SpawnCustomer), _firstCustomerDelay);
    }

    public void EndDay()
    {
        _darkness.FallAsleep();
        StartNewDay();
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
            print("Schedule is worked through");
            WaitForSleep();
            return;
        }

        SpawnCustomer();
    }

    private void WaitForSleep()
    {
        _nextDayButton.gameObject.SetActive(true);
        DayEnded.Invoke();
    }
}