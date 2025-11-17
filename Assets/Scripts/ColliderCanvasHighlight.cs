using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ColliderCanvasHighlight : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _highlightPrefab;
    [SerializeField] private bool _createCanvasIfMissing = true;
    [SerializeField] private float _canvasHeightOffset = 0.02f;

    [SerializeField] private Color _baseColor = new(1f, 0.8f, 0.2f, 0.5f);
    [SerializeField] private Color _highlightColor = new(1f, 0.8f, 0.2f, 1.0f);
    [SerializeField] private float _pulseSpeed = 2.0f;

    private Transform _canvasTransform;
    private readonly List<Image> _allImages = new();
    private readonly Dictionary<Collider, Image> _colliderToImage = new();

    private bool _pulsing;

    private void Awake()
    {
        InitCanvas();
        InitPrefab();
        BuildHighlights();

        SetAllColors(_baseColor);
        SetAllEnabled(true);
    }

    private void LateUpdate()
    {
        // В LateUpdate, чтобы учесть движение/анимацию перед пересчётом bounds
        UpdateAllRects();

        if (_pulsing)
        {
            float t = (Mathf.Sin(Time.time * _pulseSpeed) + 1f) * 0.5f;
            Color current = Color.Lerp(_baseColor, _highlightColor, t);
            SetAllColors(current);
        }
    }

    public void ShowAll()
    {
        SetAllEnabled(true);
    }

    public void HideAll()
    {
        SetAllEnabled(false);
    }

    public void StartPulse()
    {
        _pulsing = true;
        SetAllEnabled(true);
    }

    public void StopPulse()
    {
        _pulsing = false;
        SetAllColors(_baseColor);
    }

    public void ShowColliderHighlight(Collider collider)
    {
        if (collider == null) return;

        if (_colliderToImage.TryGetValue(collider, out var img) && img != null)
            img.enabled = true;
    }

    public void HideColliderHighlight(Collider collider)
    {
        if (collider == null) return;

        if (_colliderToImage.TryGetValue(collider, out var img) && img != null)
            img.enabled = false;
    }

    public void Rebuild()
    {
        ClearHighlights();
        BuildHighlights();
        UpdateAllRects();
    }

    private void InitCanvas()
    {
        if (_canvas == null && _createCanvasIfMissing)
        {
            GameObject go = new GameObject("HighlightCanvas");
            go.layer = gameObject.layer;
            go.transform.SetParent(transform, false);

            // Плоскость Canvas чуть выше теста, повернута горизонтально
            go.transform.localPosition = Vector3.up * _canvasHeightOffset;
            go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.localScale = Vector3.one;

            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = Camera.main;

            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 100f;

            go.AddComponent<GraphicRaycaster>();

            RectTransform crt = _canvas.GetComponent<RectTransform>();
            // Делаем его достаточно большим, чтобы влезли все зоны
            crt.sizeDelta = new Vector2(10f, 10f);
        }

        if (_canvas == null)
        {
            Debug.LogError("[ColliderCanvasHighlight] Canvas не задан и не создан автоматически.", this);
            enabled = false;
            return;
        }

        _canvasTransform = _canvas.transform;
    }

    private void InitPrefab()
    {
        if (_highlightPrefab != null)
        {
            _highlightPrefab.gameObject.SetActive(false);
            return;
        }

        GameObject imgGO = new GameObject("HighlightPrefab");
        imgGO.transform.SetParent(_canvasTransform, false);

        Image img = imgGO.AddComponent<Image>();
        img.color = _baseColor;
        img.raycastTarget = false;

        RectTransform rt = img.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(1f, 1f);

        _highlightPrefab = img;
        _highlightPrefab.gameObject.SetActive(false);
    }

    private void BuildHighlights()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            // Если вдруг на Canvas тоже окажется Collider — пропускаем
            if (col.GetComponentInParent<Canvas>() != null)
                continue;

            Image img = Object.Instantiate(_highlightPrefab, _canvasTransform);
            img.gameObject.name = "Highlight_" + col.gameObject.name;
            img.gameObject.SetActive(true);
            img.enabled = true;
            img.raycastTarget = false;

            RectTransform rt = img.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            _colliderToImage[col] = img;
            _allImages.Add(img);
        }

        UpdateAllRects();
    }

    private void ClearHighlights()
    {
        foreach (var img in _allImages)
        {
            if (img != null)
                Object.Destroy(img.gameObject);
        }

        _allImages.Clear();
        _colliderToImage.Clear();
    }

    private void UpdateAllRects()
    {
        if (_canvasTransform == null) return;

        foreach (var kvp in _colliderToImage)
        {
            Collider col = kvp.Key;
            Image img = kvp.Value;
            if (col == null || img == null) continue;

            UpdateImageForCollider(img, col);
        }
    }

    private void UpdateImageForCollider(Image img, Collider col)
    {
        Bounds b = col.bounds;

        // Берём центр по XZ, а Y приравниваем высоте Canvas
        Vector3 centerWorld = b.center;
        centerWorld.y = _canvasTransform.position.y;

        Vector3 centerLocal = _canvasTransform.InverseTransformPoint(centerWorld);

        RectTransform rt = img.rectTransform;

        // Позиция на плоскости Canvas в локальных координатах
        rt.localPosition = new Vector3(centerLocal.x, centerLocal.y, 0f);

        float width = b.size.x;
        float height = b.size.z;

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    private void SetAllEnabled(bool state)
    {
        foreach (var img in _allImages)
        {
            if (img != null)
                img.enabled = state;
        }
    }

    private void SetAllColors(Color color)
    {
        foreach (var img in _allImages)
        {
            if (img != null)
                img.color = color;
        }
    }
}
