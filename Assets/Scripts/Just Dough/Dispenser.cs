using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [SerializeField] private GameObject _dispensedPrefab;
    
    private void OnMouseDown()
    {
        
        GameObject spawnedPrefab = Instantiate(_dispensedPrefab, transform.position, transform.rotation);
        
        float zCord = Camera.main!.WorldToScreenPoint(transform.position).z;
        Vector3 targetPos = Utils.GetMouseWorldPos(zCord);
        
        targetPos.y = transform.position.y + 0.5f;
        spawnedPrefab.transform.position = targetPos;
    }
}
