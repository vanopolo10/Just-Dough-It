using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowPainter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MonoBehaviour _frostInput;
    [SerializeField] private Material _baseWindowMaterial;
    [SerializeField] private Material _resetMaterial;

    private Material _windowMaterial;
    private float _brushWidth;
    private float _brushHeight;

    private IFrostInput _input;
    private bool _isPainting;
    private Vector2? _lastPaintUv;
    private int _warm;

    [SerializeField] private RenderTexture _maskTexture;
    private bool _isPointerOver;

    public event Action<WindowPainter> PointerEntered;
    public event Action<WindowPainter> PointerExited;

    private void Start()
    {
        FrostManager.Instance.OnResetAll += ResetMask;
        FrostManager.Instance.Warmed += OnWarmed;
        
        if (FrostManager.Instance.BrushTexture == null)
        {
            Debug.LogError($"_brushTexture is not assigned in FrostPainter in {gameObject.name}");
            return;
        }

        _maskTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(Texture2D.whiteTexture, _maskTexture);
        _maskTexture.Create();

        GenerateMaterial();

        if (_frostInput is IFrostInput input)
            _input = input;
        else
            Debug.LogError($"WindowPainter requires a MonoBehaviour that implements IFrostInput in {gameObject.name}");
    }

    private void OnDisable()
    {
        FrostManager.Instance.OnResetAll -= ResetMask;
        FrostManager.Instance.Warmed += OnWarmed;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;

        _isPainting = false;
        _lastPaintUv = null;
        
        PointerEntered?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;

        _isPainting = false;
        _lastPaintUv = null;
        
        PointerExited?.Invoke(this);
    }
    
    private void Update()
    {
        if (!_isPointerOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            _isPainting = true;
            _lastPaintUv = null;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isPainting = false;
            _lastPaintUv = null;
        }

        if (!Input.GetMouseButton(0) || !_isPainting || _input == null) return;
        if (!_input.TryGetUv(out Vector2 uv)) return;
        
        if (uv.x is >= 0 and <= 1 && uv.y is >= 0 and <= 1)
        {
            Vector2 currentUV = uv;

            if (_lastPaintUv.HasValue)
            {
                float distance = Vector2.Distance(_lastPaintUv.Value, currentUV);
                
                if (distance < 0.5f)
                    DrawLine(_lastPaintUv.Value, currentUV);
                else
                    DrawOnFrost(currentUV);
            }
            else
            {
                DrawOnFrost(currentUV);
            }

            _lastPaintUv = currentUV;
        }
        else
        {
            _lastPaintUv = null;
        }
    }

    private void OnWarmed(int warm)
    {
        _warm = warm;
    }

    private void GenerateMaterial()
    {
        if (_baseWindowMaterial == null)
        {
            Debug.LogError($"_baseWindowMaterial is not assigned in FrostPainter in {gameObject.name}");
            return;
        }

        _windowMaterial = new Material(_baseWindowMaterial);

        if (gameObject.TryGetComponent(out Renderer renderer))
        {
            _brushWidth = FrostManager.Instance.BrushSize * _maskTexture.width;
            _brushHeight = FrostManager.Instance.BrushSize * _maskTexture.height;
            renderer.material = _windowMaterial;
        }
        else if (TryGetComponent(out RawImage image))
        {
            image.material = _windowMaterial;
            SetBrushSizeUi(image);
        }   
        else
            Debug.LogError($"WindowPainter requires a Renderer or RawImage in {gameObject.name}");

        _windowMaterial.SetTexture("_MaskTex", _maskTexture);
        _windowMaterial.SetTexture("_MainTex", FrostManager.Instance.FrostTexture);
        _windowMaterial.SetFloat("_BaseOpacity", FrostManager.Instance.BaseOpacities[_warm]);
        _windowMaterial.SetColor("_BaseColor", FrostManager.Instance.FrostColor);
    }

    private void SetBrushSizeUi(RawImage image)
    {
        RectTransform rt = image.rectTransform;
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Camera cam = image.canvas.worldCamera ?? Camera.main;
        Vector2 screenMin = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 screenMax = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        float uiW = screenMax.x - screenMin.x;
        float uiH = screenMax.y - screenMin.y;

        float pixelRadiusX = FrostManager.Instance.PixelBrushSize * _maskTexture.width / uiW;
        float pixelRadiusY = FrostManager.Instance.PixelBrushSize * _maskTexture.height / uiH;
        _brushWidth = pixelRadiusX * 2;
        _brushHeight = pixelRadiusY * 2;
    }

    private void DrawOnFrost(Vector2 uv)
    {
        if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            return;
            
        RenderTexture.active = _maskTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, _maskTexture.width, 0, _maskTexture.height);
        
        float x = uv.x * _maskTexture.width - _brushWidth / 2;
        float y = uv.y * _maskTexture.height - _brushHeight / 2;

        Rect rect = new Rect(
            Mathf.Clamp(x, 0, _maskTexture.width - _brushWidth),
            Mathf.Clamp(y, 0, _maskTexture.height - _brushHeight),
            _brushWidth, 
            _brushHeight
        );
        
        Graphics.DrawTexture(rect, FrostManager.Instance.BrushTexture);
        GL.PopMatrix();
        RenderTexture.active = null;
    }

    private void DrawLine(Vector2 from, Vector2 to)
    {
        float stepSize = _brushHeight * 0.1f;
        
        if (stepSize <= 0)
        {
            DrawOnFrost(to);
            return;
        }
        
        float distance = Vector2.Distance(from, to);
        int steps = Mathf.CeilToInt(distance / (FrostManager.Instance.BrushSize * 0.1f));
        steps = Mathf.Min(steps, 50);

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            Vector2 point = Vector2.Lerp(from, to, t);
            DrawOnFrost(point);
        }
    }

    void ResetMask()
    {
        Graphics.Blit(Texture2D.whiteTexture, _maskTexture);
        GenerateMaterial();
    }
}