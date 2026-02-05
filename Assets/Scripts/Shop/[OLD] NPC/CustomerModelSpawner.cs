using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerModelSpawner : MonoBehaviour
{
    [SerializeField] private float _respawnDelayMin = 2f;
    [SerializeField] private float _respawnDelayMax = 5f;

    private CustomerAnimatorController _animatorController;
    private Customer _customer;
    private CustomerRouteMover _routeMover;
    //private ProductComparator _productComparator;
    private Coroutine _respawnRoutine;

    private void Awake()
    {
        _routeMover = GetComponent<CustomerRouteMover>();
       // _productComparator = GetComponent<ProductComparator>();
    }

    private void OnEnable()
    {
        _routeMover.ReachedCounter += OnReachedCounter;
        _routeMover.LeftCafe += OnCustomerLeftCafe;
    }

    private void OnDisable()
    {
        _routeMover.ReachedCounter -= OnReachedCounter;
        _routeMover.LeftCafe -= OnCustomerLeftCafe;
    }

    public void MoveOut()
    {
        if (_animatorController == null) return;

        _routeMover.MoveOut();
    }

    

    public GameObject SpawnNewCustomer(GameObject prefab)
    {
        if (_respawnRoutine != null)
        {
            StopCoroutine(_respawnRoutine);
            _respawnRoutine = null;
        }

        if (_animatorController)
            Destroy(_animatorController.gameObject);

        GameObject spawnedCustomer = Instantiate(prefab, transform);
        Debug.Log("succesful spawn");

        _animatorController = spawnedCustomer.GetComponentInChildren<CustomerAnimatorController>();
        _customer = spawnedCustomer.GetComponent<Customer>();

        _routeMover.MoveIn(_animatorController.transform, _animatorController, Random.Range(0, 2));
        Debug.Log("succesful start");
        //_productComparator.SetQuery(new Query());

        return spawnedCustomer;
    }

    private void OnReachedCounter()
    { 
        _animatorController?.OnReachedCounter();
        _customer.OnReachedCounter();
    }

    private void OnCustomerLeftCafe()
    {
        //_respawnRoutine = StartCoroutine(RespawnAfterDelay());
    }

    /*
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(_respawnDelayMin, _respawnDelayMax));
        Respawn();
    }
    */
}
