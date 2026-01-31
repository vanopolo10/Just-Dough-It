using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerModelSpawner : MonoBehaviour
{
    [SerializeField] private List<CustomerAnimatorController> _customerPrefabs;
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private float _respawnDelayMin = 2f;
    [SerializeField] private float _respawnDelayMax = 5f;

    private CustomerAnimatorController _animatorController;
    private CustomerRouteMover _routeMover;
    private ProductComparator _productComparator;
    private Coroutine _respawnRoutine;

    private void Awake()
    {
        _routeMover = GetComponent<CustomerRouteMover>();
        _productComparator = GetComponent<ProductComparator>();
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

    private void Start()
    {
        Respawn();
    }

    public void Accept()
    {
        if (_animatorController == null) return;

        _routeMover.MoveOut();
    }

    public void Decline()
    {
        if (_animatorController == null) return;

        int variant = Random.Range(0, 2);
        _animatorController.OnTriggerNah(variant);
    }

    private void Respawn()
    {
        if (_respawnRoutine != null)
        {
            StopCoroutine(_respawnRoutine);
            _respawnRoutine = null;
        }

        if (_animatorController)
            Destroy(_animatorController.gameObject);

        if (_speechBubble)
            _speechBubble.SetActive(false);

        _animatorController = Instantiate(
            _customerPrefabs[Random.Range(0, _customerPrefabs.Count)],
            transform);

        if (_animatorController.TryGetComponent(out ModelRandomizer randomizer))
            randomizer.TurnAndColorMeshes();

        _routeMover.MoveIn(_animatorController.transform, _animatorController, Random.Range(0, 2));

        _productComparator.SetQuery(new Query());
    }

    private void OnReachedCounter()
    {
        if (_speechBubble)
            _speechBubble.SetActive(true);

        _animatorController?.OnReachedCounter();
    }

    private void OnCustomerLeftCafe()
    {
        _respawnRoutine = StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(_respawnDelayMin, _respawnDelayMax));
        Respawn();
    }
}
