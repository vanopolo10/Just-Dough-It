using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Dispenser : MonoBehaviour
{
    public GameObject dispensedPrefab;
    public void OnMouseDown()
    {
        
        GameObject spawnedPrefab = Instantiate(dispensedPrefab, transform.position, transform.rotation);
        float _zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
        Vector3 targetPos = Utils.GetMouseWorldPos(_zCord);
        targetPos.y = transform.position.y+0.5f;
        spawnedPrefab.transform.position = targetPos;
    }
}
