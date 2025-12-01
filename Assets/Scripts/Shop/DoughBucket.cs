using UnityEngine;

public class DoughBucket : MonoBehaviour
{
    [SerializeField] private DoughController _doughPrefab;
    [SerializeField] private Vector3 _doughSpawnPosition;
    
    private void OnMouseDown()
    {
        if (Cafe.Instance.CurrentDough != null) 
            return;
        
        DoughController doughController = Instantiate(_doughPrefab);
        doughController.SetState(DoughState.Raw);
        Cafe.Instance.SetDough(doughController);
    }
}
