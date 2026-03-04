using UnityEngine;

public class CustomerModelSpawner : MonoBehaviour
{
    public GameObject Spawn(GameObject prefab, Transform transformToSpawn)
    {
        if (!prefab)
            return null;

        return Instantiate(prefab, transformToSpawn ? transformToSpawn : transform);
    }
}