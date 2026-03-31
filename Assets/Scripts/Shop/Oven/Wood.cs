using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Wood : MonoBehaviour
{
    private static readonly int BurnProgress = Shader.PropertyToID("_BurnProgress");
    private static readonly int EmissionProgress = Shader.PropertyToID("_EmissionProgress");
    private static readonly int EmissionIntensity = Shader.PropertyToID("_EmissionIntensity");
    private static readonly int PulseSpeed = Shader.PropertyToID("_PulseSpeed");

    [Header("Visual Settings")]
    [SerializeField] private float _maxEmissionIntensity = 8f;
    [SerializeField] private float _pulseSpeed = 3f;
    
    [Header("Effects")]
    [SerializeField] private Light _fireLight;
    
    private Material _material;
    private float _originalLightIntensity;

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            _material = renderer.material;
            _material.SetFloat(BurnProgress, 0f);
            _material.SetFloat(EmissionProgress, 0f);
            _material.SetFloat(EmissionIntensity, _maxEmissionIntensity);
            _material.SetFloat(PulseSpeed, _pulseSpeed);
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
    
    public void SetVisualProgress(float burnProgress, float emissionProgress)
    {
        if (_material == null)
            return;
            
        _material.SetFloat(BurnProgress, burnProgress);
        _material.SetFloat(EmissionProgress, emissionProgress);
        
        if (_fireLight != null)
        {
            float lightIntensity = _originalLightIntensity * emissionProgress * (0.8f + Mathf.Sin(Time.time * 15f) * 0.2f);
            _fireLight.intensity = Mathf.Max(0, lightIntensity);
            _fireLight.color = Color.Lerp(new Color(1f, 0.5f, 0.2f), new Color(0.8f, 0.3f, 0.1f), burnProgress);
        }
    }
}