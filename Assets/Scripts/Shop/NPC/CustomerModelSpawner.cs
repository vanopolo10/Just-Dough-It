using System.Collections.Generic;
using UnityEngine;

public class CustomerModelSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> models;
    [SerializeField] private float respawnDelay;
    [SerializeField] private GameObject speechBubble;
    private CustomerModel current;
    private ProductComparator comparator;
    
    //public Animator door;

    public void Start()
    {
        comparator = GetComponent<ProductComparator>();
        Respawn();
    }
    public void Finish()
    {
        current?.Finish();
        Invoke("Respawn", respawnDelay);
    }
    public void Decline()
    {
        current?.Decline();
    }
    public void Respawn()
    {
        current?.Despawn();
        //door.ResetTrigger("open");
        //door.SetTrigger("open");
        int index = UnityEngine.Random.Range(0, models.Count);
        current = Instantiate(models[index], transform).GetComponent<CustomerModel>();
        current.textBubble = speechBubble;
        current.Begin();
        comparator.SetQuery(current.currentQuery.query);
    }

}
