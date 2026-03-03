using UnityEngine;

public class CustomerModelSpawner : MonoBehaviour
{
    private CustomerAnimatorController _animatorController;
    private CustomerRouteMover _routeMover;

    private void Awake()
    {
        _routeMover = GetComponent<CustomerRouteMover>();
    }
    
    public GameObject SpawnNewCustomer(GameObject prefab)
    {
        if (_animatorController)
            Destroy(_animatorController.gameObject);

        GameObject spawnedCustomer = Instantiate(prefab, transform);

        _animatorController = spawnedCustomer.GetComponentInChildren<CustomerAnimatorController>();

        _routeMover.MoveIn(_animatorController.transform, _animatorController);

        return spawnedCustomer;
    }
}
