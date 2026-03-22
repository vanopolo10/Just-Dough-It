using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 1.6f;
    [SerializeField] private float _rotateSpeed = 8f;
    [SerializeField] private float _stopDistance = 0.05f;
    [SerializeField] private float _modelForwardOffsetY = 180f;

    private Dictionary<Transform, Coroutine> _activeRoutines = new();
    
    public void Stop(Transform npc)
    {
        if (!npc) return;

        if (_activeRoutines.TryGetValue(npc, out Coroutine routine) && routine != null)
        {
            StopCoroutine(routine);
            _activeRoutines.Remove(npc);
        }
    }
    
    public void StopAll()
    {
        StopAllCoroutines();

        _activeRoutines.Clear();
    }
    
    public void MoveRoutine(IEnumerator routine)
    {
        if (routine == null)
        {
            Debug.LogError("MoveRoutine: routine is null!");
            return;
        }
    
        if (!enabled || !gameObject.activeInHierarchy)
        {
            Debug.LogError($"MoveRoutine: component not ready! enabled: {enabled}, active: {gameObject.activeInHierarchy}");
            return;
        }
    
        StartCoroutine(routine);
        Debug.Log($"MoveRoutine: started routine {routine.ToString()}");
    }
    
    public IEnumerator MoveTo(Transform target, Vector3 destination)
    {
        if (!target)
        {
            Debug.Log($"[NpcMover] MoveTo aborted for destination {destination}");
            yield break;
        }

        Debug.Log($"[NpcMover] MoveTo called for '{target.name}' to destination {destination}");

        while (Vector3.Distance(target.position, destination) > _stopDistance)
        {
            if (!target) yield break;

            Vector3 dir = (destination - target.position).normalized;
            target.position = Vector3.MoveTowards(target.position, destination, _moveSpeed * Time.deltaTime);

            RotateTowards(target, dir);
            yield return null;
        }
    }
    
    public IEnumerator FaceTo(Transform target, Vector3 lookAt)
    {
        if (!target)
        {
            Debug.Log($"[NpcMover] faceto aborted for destination {lookAt}");
            yield break;
        }

        Debug.Log($"[NpcMover] faceto called for '{target.name}' to destination {lookAt}");

        Vector3 dir = lookAt - target.position;
        if (dir.sqrMagnitude < 0.001f) yield break;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up) *
                               Quaternion.Euler(0, _modelForwardOffsetY, 0);

        while (Quaternion.Angle(target.rotation, targetRot) > 1f)
        {
            if (!target) yield break;

            target.rotation = Quaternion.Slerp(target.rotation, targetRot, _rotateSpeed * Time.deltaTime);
            yield return null;
        }

        if (target)
            target.rotation = targetRot;
    }
    
    private void RotateTowards(Transform target, Vector3 dir)
    {
        if (!target || dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up) *
                               Quaternion.Euler(0, _modelForwardOffsetY, 0);

        target.rotation = Quaternion.Slerp(target.rotation, targetRot, _rotateSpeed * Time.deltaTime);
    }
    
    private void OnDestroy()
    {
        StopAll();
    }
}