using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Wood : MonoBehaviour
{
    private static readonly int BurnProgress = Shader.PropertyToID("_BurnProgress");
    private static readonly int EmissionProgress = Shader.PropertyToID("_EmissionProgress");
    private static readonly int EmissionIntensity = Shader.PropertyToID("_EmissionIntensity");

    [SerializeField] private float _maxEmission = 8f;
    [SerializeField] private Light _fireLight;

    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;

    private float _baseLight;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        if (_fireLight != null)
            _baseLight = _fireLight.intensity;

        ResetWood();
    }

    public void ResetWood()
    {
        Apply(0f, 0f);

        if (_fireLight != null)
            _fireLight.intensity = 0f;
    }

    public void SetVisualProgress(float burn, float emission)
    {
        Apply(burn, emission);

        if (_fireLight != null)
        {
            float flicker = 0.8f + Mathf.Sin(Time.time * 15f) * 0.2f;
            _fireLight.intensity = _baseLight * emission * flicker;
        }
    }

    private void Apply(float burn, float emission)
    {
        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetFloat(BurnProgress, burn);
        _mpb.SetFloat(EmissionProgress, emission);
        _mpb.SetFloat(EmissionIntensity, emission * _maxEmission);

        _renderer.SetPropertyBlock(_mpb);
    }
}