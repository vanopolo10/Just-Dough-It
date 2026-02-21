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
        Debug.Log("succesful spawn");

        _animatorController = spawnedCustomer.GetComponentInChildren<CustomerAnimatorController>();

        _routeMover.MoveIn(_animatorController.transform, _animatorController);
        Debug.Log("succesful start");

        return spawnedCustomer;
    }
}
