using System.Collections.Generic;
using UnityEngine;

public class CustomerModelSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _models;
    [SerializeField] private float _respawnDelay;
    [SerializeField] private GameObject _speechBubble;
    
    private CustomerModel _customerModel;
    private ProductComparator _comparator;

    public void Start()
    {
        _comparator = GetComponent<ProductComparator>();
        Respawn();
    }
    
    public void Finish()
    {
        _customerModel?.Finish();
        Invoke("Respawn", _respawnDelay);
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
        int index = Random.Range(0, _models.Count);
        _customerModel = Instantiate(_models[index], transform).GetComponent<CustomerModel>();
        _customerModel.SetTextBubble(_speechBubble);
        _customerModel.Begin();
        _comparator.SetQuery(_customerModel.CurrentQuery.Query);
    }
}
