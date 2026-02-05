using NUnit.Framework;
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
    public UnityEvent OnDayStarted = new UnityEvent();
    public UnityEvent OnDayEnded = new UnityEvent();
    public UnityEvent OnCustomerSpawned = new UnityEvent();

    void Start()
    {
        _spawner = GetComponent<CustomerModelSpawner>();
        StartNewDay();
    }
    public void StartNewDay() {
        _currentIndex = 0;

        OnDayStarted.Invoke();

        SpawnCustomer();
    }
    public void SpawnCustomer() {
        GameObject prefabFromPool = _schedule[_currentIndex].GetCustomerfromPool();
        GameObject spawnedCustomer = _spawner.SpawnNewCustomer(prefabFromPool);

        _currentCustomer = spawnedCustomer.GetComponent<Customer>();

        OnCustomerSpawned.Invoke();
        Debug.Log("fully Spawned Customer");
    }
    public void NextCustomer() { 
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
    public void EndDay() {
        Debug.Log("Day ended! Starting new one in 10s");
        OnDayEnded.Invoke();

        Invoke(nameof(StartNewDay), 10f); // temp obviously
    }

    public void ResetCustomerDialogue()
    {
        _currentCustomer?.ResetDialogue();
    }
}
