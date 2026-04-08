using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manual : MonoBehaviour
{
    [SerializeField] private Vector3 _openPosition;
    [SerializeField] private Quaternion _openRotation;
    [SerializeField] private float _duration;
    [SerializeField] private float _coverRotation;
    
    [SerializeField] private GameObject _cover;
    [SerializeField] private GameObject _back;
    
    [SerializeField] private List<ManualSection> _manualSection;
    
    private Vector3 _closedPosition;
    private Quaternion _closedRotation;
    private Quaternion _closedCoverRotation;
    private Quaternion _openCoverRotation;
    
    private bool _canClick = true;
    private float _transitionProgress;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        _closedPosition = transform.position;
        _closedRotation = transform.rotation;
        _closedCoverRotation = _cover.transform.localRotation;
        _openCoverRotation = Quaternion.Euler(0, 0, _coverRotation);
    }
    
    private void OnMouseDown()
    {
        if (_canClick)
        {
            StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        _canClick = !IsOpen;
        
        Vector3 startPos = IsOpen ? _openPosition : _closedPosition;
        Vector3 targetPos = IsOpen ? _closedPosition : _openPosition;
        
        Quaternion startRot = IsOpen ? _openRotation : _closedRotation;
        Quaternion targetRot = IsOpen ? _closedRotation : _openRotation;
        
        Quaternion startCoverRot = IsOpen ? _openCoverRotation : _closedCoverRotation;
        Quaternion targetCoverRot = IsOpen ? _closedCoverRotation : _openCoverRotation;
        
        float time = 0f;

        while (time < _duration)
        {
            time += Time.deltaTime;
            _transitionProgress = Mathf.Clamp01(time / _duration);

            transform.position = Vector3.Lerp(startPos, targetPos, _transitionProgress);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, _transitionProgress);
            _cover.transform.localRotation = Quaternion.Slerp(startCoverRot, targetCoverRot, _transitionProgress);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
        _cover.transform.localRotation = targetCoverRot;
        
        _canClick = !IsOpen;
        IsOpen = !IsOpen;
    }
}