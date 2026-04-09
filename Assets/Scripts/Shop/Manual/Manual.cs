using System;
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
    [SerializeField] private Transform _leftSlot;
    [SerializeField] private Transform _rightSlot;

    [Header("Sections")]
    [SerializeField] private List<ManualSection> _manualSections;

    private Collider _coverCollider;
    private Canvas _currentLeft;
    private Canvas _currentRight;

    private int _sectionId = 0;
    private int _pageId = 0;

    private Vector3 _closedPosition;
    private Quaternion _closedRotation;
    private Quaternion _closedCoverRotation;
    private Quaternion _openCoverRotation;

    private bool _isAnimating = false;
    private bool _canOpen = true;

    public event Action Opened;
    
    public bool IsOpen { get; private set; } = false;
    public ManualSection LoreSection => _manualSections[0];

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
        if (!_isAnimating && !IsOpen && _canOpen)
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

    public void SetCanOpen(bool canOpen)
    {
        _canOpen = canOpen;
    }
    
    public void NextPage()
    {
        if (_sectionId == _manualSections.Count - 1 &&
            _pageId == _manualSections[_sectionId].Pages.Count - 1)
            return;

        if (_pageId < _manualSections[_sectionId].Pages.Count - 1)
        {
            _pageId++;
        }
        else
        {
            _sectionId++;
            _pageId = 0;
        }

        DrawPages();
    }

    public void PreviousPage()
    {
        if (_sectionId == 0 && _pageId == 0)
            return;

        if (_pageId > 0)
        {
            _pageId--;
        }
        else
        {
            _sectionId--;
            _pageId = _manualSections[_sectionId].Pages.Count - 1;
        }

        DrawPages();
    }

    public void GoToSection(int sectionId)
    {
        _sectionId = sectionId;
        _pageId = 0;
        
        DrawPages();
    }

    private void Redraw()
    {
        ClearPages();
        DrawPages();
    }

    private void DrawPages()
    {
        var pageData = _manualSections[_sectionId].Pages[_pageId];

        if (_currentLeft)
            Destroy(_currentLeft.gameObject);

        if (_currentRight)
            Destroy(_currentRight.gameObject);

        _currentLeft = Instantiate(pageData.LeftPage, _leftSlot);
        _currentLeft.transform.localPosition = Vector3.zero;
        _currentLeft.transform.localRotation = Quaternion.identity;

        _currentRight = Instantiate(pageData.RightPage, _rightSlot);
        _currentRight.transform.localPosition = Vector3.zero;
        _currentRight.transform.localRotation = Quaternion.identity;
    }

    private void ClearPages()
    {
        foreach (Transform child in _cover.transform)
            Destroy(child.gameObject);

        foreach (Transform child in _back.transform)
            Destroy(child.gameObject);
    }

    private IEnumerator Move()
    {
        _isAnimating = true;
        _coverCollider.enabled = IsOpen;

        Vector3 startPos = IsOpen ? _openPosition : _closedPosition;
        Vector3 targetPos = IsOpen ? _closedPosition : _openPosition;

        Quaternion startRot = IsOpen ? _openRotation : _closedRotation;
        Quaternion targetRot = IsOpen ? _closedRotation : _openRotation;

        Quaternion startCoverRot = IsOpen ? _openCoverRotation : _closedCoverRotation;
        Quaternion targetCoverRot = IsOpen ? _closedCoverRotation : _openCoverRotation;

        if(IsOpen == false)
            Opened?.Invoke();

        IsOpen = !IsOpen;

        float time = 0f;

        while (time < _duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _duration);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            _cover.transform.localRotation = Quaternion.Slerp(startCoverRot, targetCoverRot, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
        _cover.transform.localRotation = targetCoverRot;


        _isAnimating = false;
    }
}