using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerModelSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _speechBubble;

    [Header("Models and materials")]
    [SerializeField] private List<GameObject> _models;
    [SerializeField] private List<Material> _faceMats;
    [SerializeField] private List<Material> _clothMats;
    
    [Header("Timings")]
    [SerializeField] private float _respawnDelayMin;
    [SerializeField] private float _respawnDelayMax;
    
    private CustomerModel _customerModel;
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
        _customerModel?.Finish();
        Invoke(nameof(Respawn), Random.Range(_respawnDelayMin, _respawnDelayMax));
    }
    
    public void Decline()
    {
        _customerModel?.Decline();
    }
    
    public void Respawn()
    {
        _customerModel?.Despawn();
        //door.ResetTrigger("open");
        //door.SetTrigger("open");
        _customerModel = Instantiate(_models[Random.Range(0, _models.Count)], transform).AddComponent<CustomerModel>();
        _customerModel.SetTextBubble(_speechBubble);
        GetMeshes();
        ColorMeshes();
        _customerModel.Begin();
        //_comparator.SetQuery(_customerModel.CurrentQuery.Query);
    }

    private void ColorMeshes()
    {
        _face.material = _faceMats[Random.Range(0, _faceMats.Count)];
        
        Material coatMat = _clothMats[Random.Range(0, _faceMats.Count)];
        _coat.material = coatMat;
        _collar.material = coatMat;
        
        _scarf.material = _clothMats[Random.Range(0, _faceMats.Count)];
        _gloves.material = _clothMats[Random.Range(0, _faceMats.Count)];
        _pants.material = _clothMats[Random.Range(0, _faceMats.Count)];
        _hat.material = _clothMats[Random.Range(0, _faceMats.Count)];
    }

    private void GetMeshes()
    {
        _customerModel.transform.Find("Face").TryGetComponent(out _face);
        _customerModel.transform.Find("Face").TryGetComponent(out _hat);
        _customerModel.transform.Find("Face").TryGetComponent(out _scarf);
        _customerModel.transform.Find("Face").TryGetComponent(out _collar);
        _customerModel.transform.Find("Face").TryGetComponent(out _pants);
        _customerModel.transform.Find("Face").TryGetComponent(out _gloves);
        _customerModel.transform.Find("Face").TryGetComponent(out _coat);
    }
}
