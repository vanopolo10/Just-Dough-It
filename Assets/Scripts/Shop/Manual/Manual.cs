using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Manual : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private Vector3 _openPosition;
    [SerializeField] private Quaternion _openRotation;
    [SerializeField] private float _duration;
    [SerializeField] private float _coverRotation;
    
    [Header("Parts")]
    [SerializeField] private GameObject _cover;
    [SerializeField] private GameObject _back;

    [Header("Transforms")] 
    [SerializeField] private Transform _leftPageTransform;
    [SerializeField] private Transform _rightPageTransform;
    
    [Header("Sections")]
    [SerializeField] private List<ManualSection> _manualSection;

    private Collider _coverCollider;
    
    private int _sectionId = 0;
    private int _pageId = 0;
    
    private Vector3 _closedPosition;
    private Quaternion _closedRotation;
    private Quaternion _closedCoverRotation;
    private Quaternion _openCoverRotation;
    
    private bool _isAnimating = false;
    private float _transitionProgress;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        _coverCollider = GetComponent<Collider>();
    }
    
    private void Start()
    {
        _closedPosition = transform.position;
        _closedRotation = transform.rotation;
        _closedCoverRotation = _cover.transform.localRotation;
        _openCoverRotation = Quaternion.Euler(0, 0, _coverRotation);
        
        DrawPages();
    }
    
    private void OnMouseDown()
    {
        if (!_isAnimating && !IsOpen)
            StartCoroutine(Move());
    }
    
    public void Close()
    {
        if (!_isAnimating && IsOpen)
        {
            StopAllCoroutines();
            StartCoroutine(Move());
        }
    }

    private void DrawPages()
    {
        Canvas canvasLeft = Instantiate(_manualSection[_sectionId].Pages[_pageId].LeftPage, _cover.transform);
        canvasLeft.transform.position = _leftPageTransform.position;
        canvasLeft.transform.rotation = _leftPageTransform.rotation;
        
        Canvas canvasRight = Instantiate(_manualSection[_sectionId].Pages[_pageId].RightPage, _back.transform);
        canvasRight.transform.position = _rightPageTransform.position;
        canvasRight.transform.rotation = _rightPageTransform.rotation;
    }
    
    private IEnumerator Move()
    {
        _isAnimating = true;
        
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
        
        _coverCollider.enabled = IsOpen;
        IsOpen = !IsOpen;
        _isAnimating = false;
    }
}