using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    [Header("Click Settings")]
    [SerializeField] private bool _useLushClicks;
    [SerializeField] private ClickMaterial[] _clickMaterials;
    [SerializeField] private string _defaultClickTag = "Default";
    [SerializeField] private float _clickRayDistance = 100f;
    [SerializeField] private LayerMask _clickMask = ~0;

    [Header("Particles")]
    [SerializeField] private GameObject _particlesPrefab;

    private readonly Dictionary<string, ClickMaterialData> _materialsByTag =
        new(StringComparer.OrdinalIgnoreCase);

    private Camera _cam;
    private ParticleSystem _particlesInstance;

    private ParticleSystem.MainModule _main;
    private ParticleSystem.EmissionModule _emission;
    private ParticleSystem.TextureSheetAnimationModule _textureAnim;

    private InputAction _clickAction;

    public event Action<bool> ClicksToggled;

    private void Awake()
    {
        _cam = Camera.main;

        InitializeInput();
        InitializeParticles();
        InitializeMaterials();
    }

    private void OnEnable()
    {
        ClicksToggled += ToggleClicks;
    }

    private void OnDisable()
    {
        ClicksToggled -= ToggleClicks;

        if (_clickAction == null) return;

        _clickAction.performed -= OnClickPerformed;
        _clickAction.Disable();
    }
    
    private static Color DarkenHSV(Color color, float factor)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        v = Mathf.Clamp01(v - factor);
        return Color.HSVToRGB(h, s, v);
    }
    
    private void InitializeInput()
    {
        _clickAction = new InputAction(
            name: "LeftClick",
            binding: "<Mouse>/leftButton"
        );

        _clickAction.performed += OnClickPerformed;
        _clickAction.Enable();
    }

    private void InitializeParticles()
    {
        if (_particlesPrefab == null) return;

        _particlesInstance = Instantiate(_particlesPrefab, transform)
            .GetComponent<ParticleSystem>();

        _main = _particlesInstance.main;
        _emission = _particlesInstance.emission;
        _textureAnim = _particlesInstance.textureSheetAnimation;
    }

    private void InitializeMaterials()
    {
        if (_clickMaterials == null || _clickMaterials.Length == 0)
            return;

        _materialsByTag.Clear();

        foreach (var entry in _clickMaterials)
        {
            if (entry == null ||
                string.IsNullOrEmpty(entry.Tag) ||
                entry.MaterialData == null)
                continue;

            _materialsByTag[entry.Tag] = entry.MaterialData;
        }
    }

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        if (_useLushClicks == false || _cam == null)
            return;

        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, _clickRayDistance, _clickMask))
            return;

        string objectTag = _defaultClickTag;

        if (hit.transform.TryGetComponent(out ClickableObject clickable))
        {
            objectTag = clickable.Tag;
            clickable.PlayReactiveAnimation();
        }
        else
        {
            ClickableObject secondClickable = hit.transform.GetComponentInParent<ClickableObject>();
            if (secondClickable)
            {
                objectTag = secondClickable.Tag;
                secondClickable.PlayReactiveAnimation();
            }
        }

        if (_materialsByTag.TryGetValue(objectTag, out ClickMaterialData data) == false)
            return;

        Vector3 pos = hit.point;
        Vector3 normal = hit.normal;

        if (data.AudioClips is { Length: > 0 })
            PlayClickSound(data, pos);

        Color color = data.UseFixedColor
            ? data.ParticleColor
            : GetColorFromUV(hit, data.ColorDarkenFactor);

        PlayClickParticles(data, pos, normal, color);
    }

    private void PlayClickSound(ClickMaterialData data, Vector3 pos)
    {
        float pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        AudioClip clip = data.AudioClips[
            UnityEngine.Random.Range(0, data.AudioClips.Length)
        ];

        SoundManager.AudioEvent.OnPlaySoundIn3D?.Invoke(
            clip,
            data.Volume,
            pitch,
            pos
        );
    }

    private void PlayClickParticles(
        ClickMaterialData data,
        Vector3 position,
        Vector3 normal,
        Color color)
    {
        if (_particlesInstance == null)
            return;

        _main.startColor = color;
        _main.startSize = UnityEngine.Random.Range(
            data.MinParticleSize,
            data.MaxParticleSize
        );

        _emission.SetBurst(0, new ParticleSystem.Burst(
            0f,
            (short)UnityEngine.Random.Range(
                data.MinParticleCount,
                data.MaxParticleCount
            )
        ));

        _textureAnim.SetSprite(0, data.ParticleSprites);

        Transform t = _particlesInstance.transform;
        t.position = position;
        t.rotation = Quaternion.LookRotation(normal, Vector3.up);

        _particlesInstance.Play();
    }

    // Корректно работает только с MeshCollider и Read/Write текстурами
    private Color GetColorFromUV(RaycastHit hit, float darkenFactor)
    {
        if (hit.collider.TryGetComponent(out Renderer renderer) == false)
            return Color.white;

        Texture mainTex = renderer.material.mainTexture;

        if (mainTex is not Texture2D tex)
            return Color.white;

        Vector2 uv = hit.textureCoord;

        Color color = tex.GetPixelBilinear(uv.x, uv.y);
        return DarkenHSV(color, darkenFactor);
    }

    private void ToggleClicks(bool state)
    {
        _useLushClicks = state;
    }
    
    [Serializable]
    public class ClickMaterial
    {
        [SerializeField] private ClickMaterialData _materialData;
        [SerializeField] private string _tag;

        public ClickMaterialData MaterialData => _materialData;
        public string Tag => _tag;
    }
}