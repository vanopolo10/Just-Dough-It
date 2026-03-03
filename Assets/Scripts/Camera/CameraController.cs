using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Views")]
    [SerializeField] private List<CameraView> _views;
    [SerializeField] private float _transitionDuration = 0.7f;
    
    [Header("Mouse Look Settings")]
    [SerializeField] private bool _enableMouseLook = true;
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private float _smoothTime = 0.15f;
    [SerializeField] private Vector2 _verticalLimit = new Vector2(-30f, 70f);
    
    [Header("View-Specific Settings")]
    [SerializeField] private bool _disableMouseLookDuringTransition = true;
    
    public event Action<bool> DragAllowedChanged;
    public int ViewID { get; private set; }
    
    private float _targetRotationX;
    private float _targetRotationY;
    private float _currentRotationX;
    private float _currentRotationY;
    private float _velocityX;
    private float _velocityY;
    
    private Coroutine _transitionRoutine;
    private bool _isTransitioning;
    private bool _isMouseLookActive;
    
    private void Start()
    {
        if (_views.Count == 0)
            return;
            
        ViewID = Mathf.Clamp(ViewID, 0, _views.Count - 1);
        InitializeView(ViewID);
    }
    
    private void InitializeView(int viewID)
    {
        var view = _views[viewID];
        transform.position = view.Position;
        transform.rotation = view.Rotation;
        
        _isMouseLookActive = view.AllowMouseLook;
        
        Vector3 currentRot = transform.localEulerAngles;
        _targetRotationX = currentRot.x;
        _targetRotationY = currentRot.y;
        _currentRotationX = _targetRotationX;
        _currentRotationY = _targetRotationY;
        
        DragAllowedChanged?.Invoke(view.Type == CameraViewType.Craft);
    }
    
    private void Update()
    {
        if (_enableMouseLook && _isMouseLookActive && !_isTransitioning)
            UpdateMouseLook();
    }
    
    private void UpdateMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
        
        _targetRotationX -= mouseY;
        _targetRotationY += mouseX;
        
        _targetRotationX = Mathf.Clamp(_targetRotationX, _verticalLimit.x, _verticalLimit.y);
        
        _currentRotationX = Mathf.SmoothDamp(_currentRotationX, _targetRotationX, ref _velocityX, _smoothTime);
        _currentRotationY = Mathf.SmoothDamp(_currentRotationY, _targetRotationY, ref _velocityY, _smoothTime);
        
        transform.localEulerAngles = new Vector3(_currentRotationX, _currentRotationY, 0);
    }
    
    private void OnLeft()
    {
        if (_isTransitioning)
            return;
            
        Move(_views[ViewID].Left, TurnDirection.Left);
    }
    
    private void OnRight()
    {
        if (_isTransitioning)
            return;
            
        Move(_views[ViewID].Right, TurnDirection.Right);
    }
    
    private void OnBack()
    {
        if (_isTransitioning)
            return;
            
        Move(_views[ViewID].Back, _views[ViewID].BackTurn);
    }
    
    private void Move(CameraViewType link, TurnDirection turnDirection)
    {
        if (link == CameraViewType.None)
            return;
            
        int targetID = FindView(link);
        if (targetID == ViewID)
            return;
            
        StartTransition(targetID, turnDirection);
    }
    
    private int FindView(CameraViewType type)
    {
        int index = _views.FindIndex(v => v.Type == type);
        return index >= 0 ? index : ViewID;
    }
    
    private void StartTransition(int targetID, TurnDirection turn)
    {
        if (_transitionRoutine != null)
            StopCoroutine(_transitionRoutine);
            
        _transitionRoutine = StartCoroutine(TransitionRoutine(ViewID, targetID, turn));
    }
    
    private IEnumerator TransitionRoutine(int fromID, int toID, TurnDirection turn)
    {
        _isTransitioning = true;
        DragAllowedChanged?.Invoke(false);
        
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        Vector3 targetPos = _views[toID].Position;
        Quaternion targetRot = GetAdjustedRotation(startRot, _views[toID].Rotation, turn);
        
        float time = 0f;
        
        while (time < _transitionDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _transitionDuration);
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            
            yield return null;
        }
        
        transform.position = targetPos;
        transform.rotation = targetRot;
        
        ViewID = toID;
        _isMouseLookActive = _views[toID].AllowMouseLook;
        
        Vector3 newRot = transform.localEulerAngles;
        _targetRotationX = newRot.x;
        _targetRotationY = newRot.y;
        _currentRotationX = _targetRotationX;
        _currentRotationY = _targetRotationY;
        
        _transitionRoutine = null;
        _isTransitioning = false;
        
        DragAllowedChanged?.Invoke(_views[ViewID].Type == CameraViewType.Craft);
    }
    
    private Quaternion GetAdjustedRotation(Quaternion from, Quaternion to, TurnDirection turn)
    {
        if (turn == TurnDirection.None)
            return to;
            
        Vector3 fromEuler = from.eulerAngles;
        Vector3 toEuler = to.eulerAngles;
        
        float deltaY = Mathf.DeltaAngle(fromEuler.y, toEuler.y);
        
        if (turn == TurnDirection.Left && deltaY > 0)
            toEuler.y -= 360f;
            
        if (turn == TurnDirection.Right && deltaY < 0)
            toEuler.y += 360f;
            
        return Quaternion.Euler(toEuler);
    }
    
    public void SetViewID(int viewID)
    {
        if (_isTransitioning)
            return;
            
        ViewID = Mathf.Clamp(viewID, 0, _views.Count - 1);
        InitializeView(ViewID);
    }
    
    [Serializable]
    private struct CameraView
    {
        public Vector3 Position;
        public Vector3 RotationEuler;
        public CameraViewType Type;
        public CameraViewType Left;
        public CameraViewType Right;
        public CameraViewType Back;
        public TurnDirection BackTurn;
        
        [Header("Mouse Look Settings")]
        public bool AllowMouseLook;
        
        public Quaternion Rotation => Quaternion.Euler(RotationEuler);
    }
    
    private enum TurnDirection
    {
        None,
        Left,
        Right
    }
    
    private enum CameraViewType
    {
        None,
        Door,
        Table,
        Craft,
        Oven,
        OvenDown
    }
}