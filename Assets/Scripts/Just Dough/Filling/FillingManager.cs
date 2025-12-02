using System;
using System.Collections.Generic;
using UnityEngine;

public class FillingManager : MonoBehaviour
{
    [SerializeField] private List<FillingDisplay> _displays;
    private DoughController _controller;
    
    private void Awake()
    {
        if (_controller == null)
            _controller = GetComponentInParent<DoughController>();
    }
    
    private void OnEnable()
    {
        DisplayFilling();
    }
    
    public void SetFilling(FillingType filling) 
    { 
        _controller.SetGlobalFilling(filling);
        DisplayFilling();
    }
    
    private void DisplayFilling() 
    {
        if(_controller == null)
            return;
        
        foreach (FillingDisplay display in _displays)
        {
            display.Display.SetActive(display.Type == _controller.Filling);
        }
    }
    
    [Serializable]
    private struct FillingDisplay
    {
        [SerializeField] private FillingType _type;
        [SerializeField] private GameObject _display;

        public FillingType Type => _type;
        public GameObject Display => _display;
    }
}
