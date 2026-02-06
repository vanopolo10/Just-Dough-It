using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<CustomerPool> _schedule;
    
    private CustomerModelSpawner _spawner;
    private int _currentIndex = 0;
    private Customer _currentCustomer;
    
    public Customer CurrentCustomer => _currentCustomer;
    public List<CustomerPool> Schedule => _schedule;
    
    public UnityEvent DayStarted = new UnityEvent();
    public UnityEvent DayEnded = new UnityEvent();
    public UnityEvent CustomerSpawned = new UnityEvent();

    private void Start()
    {
        _spawner = GetComponent<CustomerModelSpawner>();
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
        GameObject prefabFromPool = _schedule[_currentIndex].GetCustomerfromPool();
        GameObject spawnedCustomer = _spawner.SpawnNewCustomer(prefabFromPool);

        _currentCustomer = spawnedCustomer.GetComponent<Customer>();

        CustomerSpawned.Invoke();
        Debug.Log("fully Spawned Customer");
    }

    public void NextCustomer()
    {
        _currentIndex++;
        
        if (_currentIndex >= _schedule.Count)
        {
            _currentIndex = 0;
            EndDay();
            return;
        }
        else
        {
            SpawnCustomer();
        }
    }

    public void EndDay()
    {
        Debug.Log("Day ended! Starting new one in 10s");
        DayEnded.Invoke();

        Invoke(nameof(StartNewDay), 10f); // temp obviously
    }

    public void ResetCustomerDialogue()
    {
        _currentCustomer?.ResetDialogue();
    }
}