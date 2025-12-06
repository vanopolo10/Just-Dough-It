using System.Collections.Generic;
using UnityEngine;

public class CustomerModelSpawner : MonoBehaviour
{
    public List<GameObject> models;
    public CustomerModel current;
    public Animator door;
    public ProductComparator comparator;
    public float respawnDelay;
    public GameObject speechBubble;

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
