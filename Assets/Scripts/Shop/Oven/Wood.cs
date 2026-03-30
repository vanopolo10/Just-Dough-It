using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Wood : MonoBehaviour
{
    private static readonly int Progress = Shader.PropertyToID("_Progress");
    private static readonly int EmissionIntensity = Shader.PropertyToID("_EmissionIntensity");

    [Header("Visual Settings")]
    [SerializeField] private float _maxEmissionIntensity = 8f;
    [SerializeField] private AnimationCurve _intensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Effects")]
    [SerializeField] private Light _fireLight;
    
    private Material _material;
    private float _originalLightIntensity;
    private bool _isBurning = true;

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            _material = renderer.material;
            _material.SetFloat(Progress, 0f);
            _material.SetFloat(EmissionIntensity, 0f);
        }
        
        if (_fireLight != null)
        {
            _originalLightIntensity = _fireLight.intensity;
        }
    }

    private void OnDestroy()
    {
        if (_material != null)
            DestroyImmediate(_material);
    }
    
    public void SetBurnProgress(float progress)
    {
        if (_material == null || !_isBurning)
            return;
            
        _material.SetFloat(Progress, progress);
        
        float intensity = _maxEmissionIntensity * _intensityCurve.Evaluate(progress);
        _material.SetFloat(EmissionIntensity, intensity);
        
        if (_fireLight != null)
        {
            _fireLight.intensity = _originalLightIntensity * (1 - progress * 0.5f) * (0.8f + Mathf.Sin(Time.time * 15f) * 0.2f);
            _fireLight.color = Color.Lerp(new Color(1f, 0.5f, 0.2f), new Color(0.8f, 0.3f, 0.1f), progress);
        }
    }
    
    public void StopBurning()
    {
        _isBurning = false;
        
        if (_fireLight != null)
        {
            _fireLight.intensity = 0f;
        }
    }
}