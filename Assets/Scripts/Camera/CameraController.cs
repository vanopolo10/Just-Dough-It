using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private List<CameraView> _views;
    [SerializeField] private float _transitionDuration = 0.7f;

    public event Action<bool> DragAllowedChanged;

    public int ViewID { get; private set; }

    private Coroutine _transitionRoutine;
    private bool _isTransitioning;

    private void Start()
    {
        if (_views.Count == 0)
            return;

        ViewID = Mathf.Clamp(ViewID, 0, _views.Count - 1);
        transform.position = _views[ViewID].Position;
        transform.rotation = _views[ViewID].Rotation;
        DragAllowedChanged?.Invoke(_views[ViewID].Type == CameraViewType.Craft);
    }

    public void SetViewID(int viewID)
    {
        ViewID = Mathf.Clamp(viewID, 0, _views.Count - 1);
        transform.position = _views[ViewID].Position;
        transform.rotation = _views[ViewID].Rotation;
        DragAllowedChanged?.Invoke(_views[ViewID].Type == CameraViewType.Craft);
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
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CameraView))]
    public class CameraViewDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUI.GetPropertyHeight(property, label);
            EditorGUI.PropertyField(position, property, label, true);
        
            position.y += position.height + 2f;
            position.height = EditorGUIUtility.singleLineHeight;
        
            if (GUI.Button(position, "Move Camera Here"))
            {
                var controller = property.serializedObject.targetObject as CameraController;
                var pos = property.FindPropertyRelative("Position").vector3Value;
                var rot = property.FindPropertyRelative("RotationEuler").vector3Value;

                if (controller != null)
                {
                    controller.transform.position = pos;
                    controller.transform.rotation = Quaternion.Euler(rot);
                }

                if (Camera.main != null)
                {
                    Camera.main.transform.position = pos;
                    Camera.main.transform.rotation = Quaternion.Euler(rot);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label) + 
                   EditorGUIUtility.singleLineHeight + 2f;
        }
    }
#endif
}