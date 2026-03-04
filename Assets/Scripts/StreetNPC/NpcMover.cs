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
        foreach (var routine in _activeRoutines.Values)
        {
            if (routine != null)
                StopCoroutine(routine);
        }
        _activeRoutines.Clear();
    }
    
    public void MoveRoutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
    
    public IEnumerator MoveTo(Transform target, Vector3 destination)
    {
        if (!target) yield break;

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
        if (!target) yield break;

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