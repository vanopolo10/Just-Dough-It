using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JustDough;

[RequireComponent(typeof(RectTransform))]
public class CraftZone : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerUpHandler
{
    [Serializable]
    private class DragRule
    {
        [SerializeField] private CraftZone _fromCraftZone;
        [SerializeField] private DoughCraftAction _action;

        public CraftZone FromCraftZone => _fromCraftZone;
        public DoughCraftAction Action => _action;
    }
    
    [Header("Контроллер крафта")]
    [SerializeField] private DoughController _controller;

    [Header("Действия по клику")]
    [SerializeField] private DoughCraftAction _rightClickAction = DoughCraftAction.None;

    [Header("Роль в драг-жесте")]
    [SerializeField] private bool _isDragStartZone = false;
    [SerializeField] private bool _isDragEndZone = false;
    
    [Header("Действия по ДРАГУ (когда завершаем драг НА ЭТОЙ зоне)")]
    [SerializeField] private List<DragRule> _dragRules = new();
    
    [Header("Комбо")]
    [SerializeField] private bool _isComboZone = false;

    [Header("Визуал")]
    [SerializeField] private Image _image;
    [SerializeField] private Color _defaultColor = new(1f, 1f, 1f, 0.1f);
    [SerializeField] private Color _pressedColor;
    [SerializeField] private Color _hoverColor = new(1f, 0.9f, 0.4f, 0.4f);

    private static CraftZone _dragStartZone;
    private static bool _dragActive;
    
    private RectTransform _rectTransform;
    private bool _isPointerOver;
    private bool _isPressed;

    public bool IsComboZone => _isComboZone;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (_image == null)
            _image = GetComponent<Image>();

        if (_image != null)
        {
            _image.raycastTarget = true;
            _image.color = _defaultColor;
        }

        if (_controller == null)
            _controller = GetComponentInParent<DoughController>();
    }

    private void OnValidate()
    {
        if (_image == null)
            _image = GetComponent<Image>();

        if (_image != null)
            _image.color = _defaultColor;
    }

    public void SetColor(Color color)
    {
        if (_image != null)
            _image.color = color;
    }

    public void SetVisible(bool value)
    {
        if (_image != null)
            _image.enabled = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_controller == null)
            return;

        if (_dragStartZone != null)
            return;
        
        if (_rightClickAction == DoughCraftAction.None)
            return;

        if (eventData.button != PointerEventData.InputButton.Right)
            return;
        
        bool applied = _controller.ApplyAction(_rightClickAction, this);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;

        if (_isDragStartZone == false)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _dragStartZone = this;
            _dragActive = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;
        
        if (_isDragEndZone == false)
            return;
        
        if (_dragActive == false || _dragStartZone == null || _dragStartZone == this || !Input.GetMouseButton(0))
            return;

        HandleDragFrom(_dragStartZone);
        ClearDrag();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        
        if (_dragActive && eventData.button == PointerEventData.InputButton.Left)
            ClearDrag();
    }

    private static void ClearDrag()
    {
        _dragStartZone = null;
        _dragActive = false;
    }

    private void HandleDragFrom(CraftZone fromZone)
    {
        if (fromZone == null || _controller == null)
            return;
        
        foreach (DragRule rule in _dragRules)
        {
            if (rule == null)
                continue;

            if (rule.FromCraftZone == fromZone)
            {
                DoughCraftAction action = rule.Action;
                if (action == DoughCraftAction.None)
                    continue;

                bool applied = _controller.ApplyAction(action, this);
                Debug.Log($"[CraftZone] Drag {fromZone.name} -> {name}, action={action}, applied={applied}");
                return;
            }
        }

        Debug.Log($"[CraftZone] No drag rule for {fromZone.name} -> {name}");
    }

    
}
