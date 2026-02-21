using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CustomerRouteMover))]
public class CustomerManager : MonoBehaviour
{
    private const float HundredPercent = 100f;
    
    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private List<CustomerPool> _schedule;

    private CustomerRouteMover _customerRouteMover;
    private CustomerModelSpawner _spawner;
    private int _currentIndex = 0;
    private Customer _currentCustomer;
    
    public Customer CurrentCustomer => _currentCustomer;
    public List<CustomerPool> Schedule => _schedule;
    
    public UnityEvent DayStarted = new();
    public UnityEvent DayEnded = new();
    public UnityEvent CustomerSpawned = new();

    private void Awake()
    {
        _spawner = GetComponent<CustomerModelSpawner>();
        _customerRouteMover = GetComponent<CustomerRouteMover>();
    }

    private void OnEnable()
    {
        _customerRouteMover.ReachedCounter += InitializeCustomer;
        _customerRouteMover.LeftCafe += NextCustomer;
    }
    
    private void OnDisable()
    {
        _customerRouteMover.ReachedCounter -= InitializeCustomer;
        _customerRouteMover.LeftCafe -= NextCustomer;
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

    public void SpawnCustomer()
    {
        _worldTime.StartSmoothAddPercent(HundredPercent / _schedule.Count);
        
        GameObject prefabFromPool = _schedule[_currentIndex].GetCustomerFromPool();
        GameObject spawnedCustomer = _spawner.SpawnNewCustomer(prefabFromPool);

        _currentCustomer = spawnedCustomer.GetComponent<Customer>();

        CustomerSpawned.Invoke();
        print("fully Spawned Customer");
    }

    public void NextCustomer()
    {
        _currentCustomer.Despawn();
        _currentIndex++;
        
        if (_currentIndex >= _schedule.Count)
        {
            _currentIndex = 0;
            EndDay();
        }
        else
        {
            SpawnCustomer();
        }
    }

    public void EndDay()
    {
        print("Day ended! Starting new one in 10s");
        DayEnded.Invoke();

        Invoke(nameof(StartNewDay), 10f); // temp obviously
    }

    public void ResetCustomerDialogue()
    {
        _currentCustomer?.ResetDialogue();
    }

    private void InitializeCustomer()
    {
        _currentCustomer.OnReachedCounter();
    }
}