using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(AudioSource), typeof(AudioLowPassFilter))]
public class Wood : MonoBehaviour
{
    private static readonly int BurnProgress = Shader.PropertyToID("_BurnProgress");
    private static readonly int EmissionProgress = Shader.PropertyToID("_EmissionProgress");
    private static readonly int EmissionIntensity = Shader.PropertyToID("_EmissionIntensity");

    [SerializeField] private float _maxEmission = 8f;
    [SerializeField] private Light _fireLight;
    [SerializeField] private AudioClip _hitClip;

    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;

    private float _baseLight;

    private AudioSource _audioSource;
    private AudioLowPassFilter _audioFilter;
    private Vector3 _initialScale;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        if (_fireLight != null)
            _baseLight = _fireLight.intensity;

        _initialScale = transform.localScale;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioFilter = GetComponent<AudioLowPassFilter>();

        ResetWood();
    }

    public void ResetWood()
    {
        Apply(0f, 0f);

        if (_fireLight != null)
            _fireLight.intensity = 0f;
    }

    public void SetVisualProgress(float burn, float emission, float hatchOpenPercentage)
    {
        Apply(burn, emission);

        if (_fireLight != null)
        {
            float flicker = 0.8f + Mathf.Sin(Time.time * 15f) * 0.2f;
            _fireLight.intensity = _baseLight * emission * flicker;
        }

        _audioFilter.cutoffFrequency = Mathf.Lerp(2000, 22000, hatchOpenPercentage);
        
        if (Mathf.Approximately(emission, 1))
            transform.localScale = Vector3.Lerp(_initialScale, _initialScale * 0.5f, burn / 2f);
    }

    private void Apply(float burn, float emission)
    {
        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetFloat(BurnProgress, burn);
        _mpb.SetFloat(EmissionProgress, emission);
        _mpb.SetFloat(EmissionIntensity, emission * _maxEmission);

        _renderer.SetPropertyBlock(_mpb);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 18f);
        _audioSource.PlayOneShot(_hitClip, volume);
    }
}