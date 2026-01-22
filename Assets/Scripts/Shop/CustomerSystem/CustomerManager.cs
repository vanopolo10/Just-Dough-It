using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    private Customer _currentCustomer;
    public Customer CurrentCustomer => _currentCustomer;

    public GameObject TmpCustomerPrefab; // temporary

    void Start()
    {
        SpawnCustomer();
    }
    public void SpawnCustomer() {
        GameObject customer = Instantiate(TmpCustomerPrefab, _spawnPoint.position, _spawnPoint.rotation, transform);

        _currentCustomer = customer.GetComponent<Customer>();
    }
    public void StartNewCycle() { 
        SpawnCustomer();
    }
}
