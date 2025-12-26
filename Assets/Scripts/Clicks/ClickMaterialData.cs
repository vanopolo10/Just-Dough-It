using UnityEngine;

[CreateAssetMenu(fileName = "ClickMaterial", menuName = "ScriptableObjects/ClickMaterial")]
public class ClickMaterialData : ScriptableObject
{
    [SerializeField] private float _delay;
    [Header("Sound effects")]
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private float _volume;
    [Header("Particles")]
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private bool _useFixedColor;
    [SerializeField] private float _colorDarkenFactor = 0.2f;
    [SerializeField] private Color _particleColor;
    [SerializeField] private Sprite _particleSprites;
    [SerializeField] private float _minParticleSize = 0.5f;
    [SerializeField] private float _maxParticleSize = 1f;
    [SerializeField] private int _minParticleCount = 3;
    [SerializeField] private int _maxParticleCount = 5;

    public float Delay => _delay;
    public AudioClip[] AudioClips => _audioClips;
    public float Volume => _volume;
    public GameObject ParticlePrefab => _particlePrefab;
    public bool UseFixedColor => _useFixedColor;
    public float ColorDarkenFactor => _colorDarkenFactor;
    public Color ParticleColor => _particleColor;
    public Sprite ParticleSprites => _particleSprites;
    public float MinParticleSize => _minParticleSize;
    public float MaxParticleSize => _maxParticleSize;
    public int MinParticleCount => _minParticleCount;
    public int MaxParticleCount => _maxParticleCount;
}
