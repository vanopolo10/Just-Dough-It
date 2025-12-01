using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class FillingManager : MonoBehaviour
{
    [Serializable]
    private class FillingDisplay 
    {
        public FillingType type;
        public GameObject display;
    }

    private DoughController _controller;
    [SerializeField] private List<FillingDisplay> displays;
    private void Awake()
    {
        if (_controller == null)
            _controller = GetComponentInParent<DoughController>();
        DisplayFilling();
    }
    public void DisplayFilling() 
    {
        foreach (FillingDisplay display in displays)
        {
            if (display.type == _controller.Filling)
            {
                display.display.SetActive(true);
            }
            else display.display.SetActive(false);
        }
    }
    public void SetFilling(FillingType filling) 
    { 
        _controller.SetGlobalFilling(filling);
        DisplayFilling();
    }
}
