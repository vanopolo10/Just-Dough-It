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
    IPointerUpHandler,
    IDragHandler
{
    [Serializable]
    private struct DragRule
    {
        public CraftZone EndZone;
        public DoughCraftAction Action;
    }

    [Header("Контроллер крафта")]
    [SerializeField] private DoughController _controller;

    [Header("Действия по клику (ПКМ)")]
    [SerializeField] private DoughCraftAction _rightClickAction = DoughCraftAction.None;

    [Header("Роль в драг-жесте")]
    [SerializeField] private bool _isDragStartZone = false;
    [SerializeField] private bool _isDragEndZone = false;

    [Header("Действия по ДРАГУ (когда завершаем драг НА ЭТОЙ зоне)")]
    [SerializeField] private List<DragRule> _dragRules = new();

    [Header("Комбо")]
    [SerializeField] private bool _isComboZone = false;

    [Header("Идеальные зоны")]
    [SerializeField] private RectTransform _perfectClickArea;
    [SerializeField] private RectTransform _perfectDragArea;

    private static CraftZone _dragStartZone;
    private static bool _dragActive;
    private static bool _dragPerfect;

    private RectTransform _rectTransform;
    private bool _isPointerOver;
    private bool _isPressed;
    private bool _comboUsed;

    public bool IsComboZone => _isComboZone;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (_controller == null)
            _controller = GetComponentInParent<DoughController>();
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

        if (_isComboZone && _comboUsed)
            return;

        bool isPerfect = IsInPerfectClickArea(eventData);

        bool applied = _controller.ApplyAction(_rightClickAction, this, isPerfect);
        Debug.Log($"[CraftZone] RightClick {name}, action={_rightClickAction}, perfect={isPerfect}, applied={applied}");

        if (_isComboZone && applied)
            DisableComboZone();
    }

    private void DisableComboZone()
    {
        _comboUsed = true;

        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;

        if (_isDragStartZone == false)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _dragStartZone = this;
        _dragActive = true;
        _dragPerfect = IsInPerfectDragArea(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;

        if (_isDragEndZone == false)
            return;

        if (_dragActive == false || _dragStartZone == null || _dragStartZone == this || !Input.GetMouseButton(0))
            return;

        DoughCraftAction action = DoughCraftAction.None;

        foreach (DragRule rule in _dragRules)
        {
            if (rule.EndZone == _dragStartZone)
            {
                action = rule.Action;
                break;
            }
        }

        if (action == DoughCraftAction.None)
            return;

        bool isPerfect = _dragPerfect;
        bool applied = _controller.ApplyAction(action, this, isPerfect);
        Debug.Log($"[CraftZone] Drag {name}, action={action}, perfect={isPerfect}, applied={applied}");

        _dragActive = false;
        _dragStartZone = null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        _dragActive = false;
        _dragStartZone = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragActive == false)
            return;

        if (_dragStartZone != this)
            return;

        if (_dragPerfect == false)
            return;

        if (IsInPerfectDragArea(eventData))
            return;

        _dragPerfect = false;
    }

    private bool IsInPerfectClickArea(PointerEventData eventData)
    {
        if (_perfectClickArea == null)
            return false;

        Camera cam = eventData.pressEventCamera ?? eventData.enterEventCamera ?? Camera.main;

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _perfectClickArea,
            eventData.position,
            cam,
            out Vector2 local) && _perfectClickArea.rect.Contains(local);
    }

    private bool IsInPerfectDragArea(PointerEventData eventData)
    {
        if (_perfectDragArea == null)
            return false;

        Camera cam = eventData.pressEventCamera ?? eventData.enterEventCamera ?? Camera.main;

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _perfectDragArea,
            eventData.position,
            cam,
            out Vector2 local) && _perfectDragArea.rect.Contains(local);
    }
}
