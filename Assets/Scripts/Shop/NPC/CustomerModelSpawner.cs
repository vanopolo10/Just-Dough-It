using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerModelSpawner : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("ОБЪЕКТ bubble на сцене (один), который включается, когда клиент у стойки.")]
    [SerializeField] private GameObject _speechBubble;

    [Header("Models & materials")]
    [SerializeField] private List<CustomerModel> _models;
    [SerializeField] private List<Material>      _faceMats;
    [SerializeField] private List<Material>      _clothMats;

    [Header("Timings")]
    [SerializeField] private float _respawnDelayMin = 2f;
    [SerializeField] private float _respawnDelayMax = 5f;

    private CustomerModel _customerModel;
    private CustomerRouteMover _routeMover;
    private ProductComparator _comparator;

    private SkinnedMeshRenderer _face;
    private SkinnedMeshRenderer _hat;
    private SkinnedMeshRenderer _scarf;
    private SkinnedMeshRenderer _collar;
    private SkinnedMeshRenderer _pants;
    private SkinnedMeshRenderer _gloves;
    private SkinnedMeshRenderer _coat;

    private void Start()
    {
        _comparator = GetComponent<ProductComparator>();
        Respawn();
    }

    public void Finish()
    {
        Debug.Log("[Spawner] Finish (Accept)");

        if (_customerModel != null)
            _customerModel.Finish();
        else
            Debug.LogWarning("[Spawner] Finish called but _customerModel is null.");

        Invoke(nameof(Respawn), Random.Range(_respawnDelayMin, _respawnDelayMax));
    }

    public void Decline()
    {
        Debug.Log("[Spawner] Decline (Wrong product)");

        if (_customerModel != null)
            _customerModel.Decline();
        else
            Debug.LogWarning("[Spawner] Decline called but _customerModel is null.");
    }

    public void Respawn()
    {
        Debug.Log("[Spawner] Respawn");

        if (_routeMover != null)
        {
            _routeMover.ReachedCounter -= OnCustomerReachedCounter;
            _routeMover.LeftCafe       -= OnCustomerLeftCafe;
        }

        if (_customerModel != null)
            _customerModel.Despawn();

        if (_speechBubble != null)
            _speechBubble.SetActive(false);

        if (_models == null || _models.Count == 0)
        {
            Debug.LogError("[Spawner] No customer models assigned.");
            return;
        }

        _customerModel = Instantiate(_models[Random.Range(0, _models.Count)], transform);

        if (_speechBubble != null)
            _customerModel.SetTextBubble(_speechBubble);
        else
            Debug.LogError("[Spawner] Speech bubble object is not assigned in inspector.");

        _routeMover = _customerModel.GetComponent<CustomerRouteMover>();

        if (_routeMover != null)
        {
            _routeMover.ReachedCounter += OnCustomerReachedCounter;
            _routeMover.LeftCafe       += OnCustomerLeftCafe;
            _routeMover.StartEntry();
        }
        else
        {
            Debug.LogError("[Spawner] CustomerRouteMover not found on customer model.");
        }

        //GetMeshes();
        //ColorMeshes();
    }

    private void OnCustomerReachedCounter(CustomerRouteMover mover)
    {
        Debug.Log("[Spawner] OnCustomerReachedCounter ? вызываю Begin()");

        if (mover != _routeMover)
            return;

        if (_customerModel != null)
            _customerModel.Begin();

        if (_comparator != null)
            _comparator.SetQuery(_customerModel.CurrentQuery.Query);
        else
            Debug.LogWarning("[Spawner] ProductComparator not found.");
    }

    private void OnCustomerLeftCafe(CustomerRouteMover mover)
    {
        Debug.Log("[Spawner] OnCustomerLeftCafe");

        if (mover != _routeMover)
            return;

        if (_speechBubble != null)
            _speechBubble.SetActive(false);

        _routeMover.ReachedCounter -= OnCustomerReachedCounter;
        _routeMover.LeftCafe       -= OnCustomerLeftCafe;
        _routeMover = null;
    }

    private void ColorMeshes()
    {
        if (_face != null && _faceMats.Count > 0)
            _face.material = _faceMats[Random.Range(0, _faceMats.Count)];

        if (_clothMats == null || _clothMats.Count == 0)
            return;

        Material coatMat = _clothMats[Random.Range(0, _clothMats.Count)];

        if (_coat   != null) _coat.material   = coatMat;
        if (_collar != null) _collar.material = coatMat;

        if (_scarf  != null) _scarf .material = _clothMats[Random.Range(0, _clothMats.Count)];
        if (_gloves != null) _gloves.material = _clothMats[Random.Range(0, _clothMats.Count)];
        if (_pants  != null) _pants .material = _clothMats[Random.Range(0, _clothMats.Count)];
        if (_hat    != null) _hat   .material = _clothMats[Random.Range(0, _clothMats.Count)];
    }

    private void GetMeshes()
    {
        _customerModel.transform.Find("Face")   ?.TryGetComponent(out _face);
        _customerModel.transform.Find("Hat")    ?.TryGetComponent(out _hat);
        _customerModel.transform.Find("Scarf")  ?.TryGetComponent(out _scarf);
        _customerModel.transform.Find("Collar") ?.TryGetComponent(out _collar);
        _customerModel.transform.Find("Pants")  ?.TryGetComponent(out _pants);
        _customerModel.transform.Find("Gloves") ?.TryGetComponent(out _gloves);
        _customerModel.transform.Find("Coat")   ?.TryGetComponent(out _coat);
    }
}
