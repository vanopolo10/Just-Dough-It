using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [SerializeField] private Filling _dispensedPrefab;
    [SerializeField] private Vector3 _spawnPoint;

    private bool _canSpawn = true;
    private Filling _filling;
    
    private void OnMouseDown()
    {
        if (_canSpawn == false)
            return;
        
        _filling = Instantiate(_dispensedPrefab, _spawnPoint, transform.rotation);
        _filling.Destroyed += OnFillingDestroyed;
        _canSpawn = false;
    }

    private void OnFillingDestroyed()
    {
        _filling.Destroyed -= OnFillingDestroyed;
        _canSpawn = true;
    }
}
