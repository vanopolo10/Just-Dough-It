using UnityEngine;

public class DoughSettleHandler : MonoBehaviour
{
    private Material _material;
    private bool _running = false;
    private float _builtUp = 0;
    private void OnEnable()
    {
        _material = GetComponent<Renderer>().material;
        _running = true;
    }
    private void Update()
    {
        if (_running)
        {
            _builtUp += Time.deltaTime;
            _material.SetFloat("_TimePassed", _builtUp);
        }
    }
}
