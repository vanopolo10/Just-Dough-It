using UnityEngine;
using UnityEngine.UI;

public class WindowPainter : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _frostInput;
    [SerializeField] private Material _baseWindowMaterial;
    [SerializeField] private Material _resetMaterial;
    [SerializeField] private Texture2D _frostTexture;
    [SerializeField] private Texture2D _brushTexture;
    [SerializeField] private float _pixelBrushSize = 50f;
    [SerializeField] private float _brushSize = 0.1f;

    [SerializeField] private float _baseOpacity;
    [SerializeField] private Color _frostColor;
    [SerializeField] private float _resetDuration = 1f;

    private Material _windowMaterial;
    private float _brushWidth;
    private float _brushHeight;

    private IFrostInput _input;
    private bool _isPainting = false;
    private Vector2? _lastPaintUv = null;
    [SerializeField] private RenderTexture _maskTexture;

    private void OnEnable()
    {
        FrostManager.OnResetAll += ResetMask;
    }

    private void OnDisable()
    {
        FrostManager.OnResetAll -= ResetMask;
    }

    private void Awake()
    {
        if(_brushTexture == null)
        {
            Debug.LogError($"_brushTexture is not assigned in FrostPainter in {gameObject.name}");
            return;
        }

        _maskTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(Texture2D.whiteTexture, _maskTexture);
        _maskTexture.Create();

        GenerateMaterial();

        if (_frostInput is IFrostInput)
            _input = _frostInput as IFrostInput;
        else
            Debug.LogError($"WindowPainter requires a MonoBehaviour that implements IFrostInput in {gameObject.name}");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _isPainting = true;
            _lastPaintUv = null;
        }

        if (Input.GetMouseButton(0) && _isPainting && _input != null)
        {
            if(_input.TryGetUv(out Vector2 uv))
            {
                Vector2 currentUV = uv;

                if (_lastPaintUv.HasValue)
                {
                    DrawLine(_lastPaintUv.Value, currentUV);
                }
                else
                {
                    DrawOnFrost(currentUV);
                }

                _lastPaintUv = currentUV;
            }
        }
    }

    private void GenerateMaterial()
    {
        if (_baseWindowMaterial == null)
        {
            Debug.LogError($"_baseWindowMaterial is not assigned in FrostPainter in {gameObject.name}");
            return;
        }

        _windowMaterial = new Material(_baseWindowMaterial);

        if (gameObject.TryGetComponent<Renderer>(out Renderer renderer))
        {
            _brushWidth = _brushSize * _maskTexture.width;
            _brushHeight = _brushSize * _maskTexture.height;
            renderer.material = _windowMaterial;
        }
        else if (TryGetComponent<RawImage>(out RawImage image))
        {
            image.material = _windowMaterial;
            SetBrushSizeUi(image);
        }   
        else
            Debug.LogError($"WindowPainter requires a Renderer or RawImage in {gameObject.name}");

        _windowMaterial.SetTexture("_MaskTex", _maskTexture);
        _windowMaterial.SetTexture("_MainTex", _frostTexture);
        _windowMaterial.SetFloat("_BaseOpacity", _baseOpacity);
        _windowMaterial.SetColor("_BaseColor", _frostColor);
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

        float pixelRadiusX = _pixelBrushSize * _maskTexture.width / uiW;
        float pixelRadiusY = _pixelBrushSize * _maskTexture.height / uiH;
        _brushWidth = pixelRadiusX * 2;
        _brushHeight = pixelRadiusY * 2;
    }

    private void DrawOnFrost(Vector2 uv)
    {
        RenderTexture.active = _maskTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, _maskTexture.width, 0, _maskTexture.height);
        Rect rect = new Rect(uv.x * _maskTexture.width - _brushWidth / 2, uv.y * _maskTexture.height - _brushHeight / 2, _brushWidth, _brushHeight);
        
        Graphics.DrawTexture(rect, _brushTexture);
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
        int steps = Mathf.CeilToInt(distance / (_brushSize * 0.1f));
        steps = Mathf.Max(steps, 1, 500);

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
        //StartCoroutine(SmoothResetMask());
    }

/*
    public IEnumerator SmoothResetMask()
    {
        float elapsed = 0f;
        Material blendMaterial = new Material(_resetMaterial);
        Texture2D whiteTex = Texture2D.whiteTexture;

        RenderTexture mask = RenderTexture.GetTemporary(_maskTexture.width, _maskTexture.height, 0, _maskTexture.format);

        while (elapsed < _resetDuration)
        {
            float t = elapsed / _resetDuration;
            blendMaterial.SetFloat("_Blend", t);
            Graphics.Blit(_maskTexture, mask, blendMaterial,-1);
            Graphics.Blit(mask, _maskTexture);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Graphics.Blit(whiteTex, _maskTexture);

        RenderTexture.ReleaseTemporary(mask);
        Destroy(blendMaterial);
    }
*/
}
