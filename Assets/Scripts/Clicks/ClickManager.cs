using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private bool _useLushClicks;
    [SerializeField] private ClickMaterial[] _clickMaterials;
    [SerializeField] private string _defaultClickTag;
    [SerializeField] private float _clickRayDistance;
    [SerializeField] private LayerMask _clickMask = ~0;
    [Header("Particles")]
    [SerializeField] private GameObject _particlesPrefab;

    private Dictionary<string, ClickMaterialData> _materialsByTag;
    private Camera _cam;
    private ParticleSystem _particlesInstance;

    private InputAction _clickAction;
    public event Action<bool> ClicksToggled;

    private void OnDisable()
    {
        ClicksToggled -= ToggleClicks;

        if (_clickAction != null)
        {
            _clickAction.performed -= OnClickPerformed;
            _clickAction.Disable();
        }
    }

    private void OnEnable()
    {
        ClicksToggled += ToggleClicks;
        _clickAction.performed += OnClickPerformed;
    }

    private void Awake()
    {
        _cam = Camera.main;
        _clickAction ??= new InputAction("LeftClick", binding: "<Mouse>/leftButton");
        _clickAction.Enable();

        _particlesInstance = Instantiate(_particlesPrefab, gameObject.transform).GetComponent<ParticleSystem>();

        if (_clickMaterials != null)
            SetDicitonary();
    }

    private void SetDicitonary()
    {
        _materialsByTag = new Dictionary<string, ClickMaterialData>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in _clickMaterials)
        {
            var tag = entry.Tag;
            var data = entry.MaterialData;
            _materialsByTag.Add(tag, data);
        }
    }    

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {

        if (_useLushClicks == false) return;

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Color color = Color.white;

        if (Physics.Raycast(ray, out RaycastHit hit, _clickRayDistance, _clickMask))
        {
            string objectTag = _defaultClickTag;

            if (hit.transform.gameObject.TryGetComponent(out ClickableObject clickableObject))
            {
                objectTag = clickableObject.ReturnTag();
                clickableObject.PlayReactiveAnimation();
            }

            Vector3 clickPosition = hit.point;
            Vector3 normal = hit.normal;
            ClickMaterialData data = null;

            if (_materialsByTag.TryGetValue(objectTag, out data))
            {
                if (data.AudioClips != null)
                    PlayClickSound(data, clickPosition);

                if (data.UseFixedColor)
                    color = data.ParticleColor;
                else
                    color = GetColorFromUV(hit, data.ColorDarkenFactor);

                PlayClickParticles(data, clickPosition, normal, color);
            }               
        }
    }

    private void PlayClickSound(ClickMaterialData data, Vector3 pos)
    {
        
        float pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        AudioClip clip = data.AudioClips[UnityEngine.Random.Range(0, data.AudioClips.Length)];
        SoundManager.AudioEvent.OnPlaySoundIn3D?.Invoke(clip, data.Volume, pitch, pos);
    }
    private void PlayClickParticles(ClickMaterialData data, Vector3 newPos, Vector3 normal, Color color)
    {
        var main = _particlesInstance.main;
        main.startColor = color;
        main.startSize = UnityEngine.Random.Range(data.MinParticleSize, data.MaxParticleSize);
        _particlesInstance.emission.SetBurst(90, new ParticleSystem.Burst(0f, (short)UnityEngine.Random.Range(data.MinParticleCount, data.MaxParticleCount)));
        _particlesInstance.textureSheetAnimation.SetSprite(0, data.ParticleSprites);
        _particlesInstance.transform.position = newPos;
        _particlesInstance.transform.rotation = Quaternion.LookRotation(normal, Vector3.up);
        _particlesInstance.Play();
    }

    // Работает как надо только с мэшколайдерами, с другими типами колайдеров цвет получается неправильный :(
    private Color GetColorFromUV(RaycastHit hit, float colorDarkenFactor)
    {
        Color color = Color.white;
        var renderer = hit.collider.GetComponent<Renderer>();

        if (renderer != null && renderer.material != null && renderer.material.mainTexture is Texture2D tex)
        {
            Vector2 uv = hit.textureCoord;

            int x = Mathf.FloorToInt(uv.x * tex.width);
            int y = Mathf.FloorToInt(uv.y * tex.height);

            color = tex.GetPixel(x, y);
            color = DarkenHSV(color, colorDarkenFactor);
        }

        return color;
    }

    Color DarkenHSV(Color color, float factor)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        v = Mathf.Clamp01(v - factor);
        return Color.HSVToRGB(h, s, v);
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
