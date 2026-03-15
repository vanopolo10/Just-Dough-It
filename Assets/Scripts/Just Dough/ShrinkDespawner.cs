using UnityEngine;

public class ShrinkDespawner : MonoBehaviour
{
    [SerializeField] private AnimationCurve _shrinkCurve;
    [SerializeField] private float _despawnTime = 1f;

    private bool _isDespawning = false;
    private float _timeLeft = 0;
    private Vector3 _recordedScale;
    public void DespawnSelf() { 
        _timeLeft = _despawnTime;
        _isDespawning = true;
        _recordedScale = transform.localScale;
    }

    private void Update()
    {
        if (_isDespawning) { 
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0)
            {
                Destroy(gameObject);
                return;
            }

            transform.localScale = _recordedScale * _shrinkCurve.Evaluate( 1 - (_timeLeft / _despawnTime) );
        }
    }
}
