using System;
using System.Collections.Generic;
using UnityEngine;

public class DaysProgression : MonoBehaviour
{
    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private List<DayObjectsList> _daysObjects;

    private int _currentDay = -1;
    
    private void Awake()
    {
        foreach (var days in  _daysObjects)
            SetDayObjectsActive(days.Objects, false);
    }

    private void OnEnable()
    {
        _customerManager.DayStarted += ChangeDay;
    }

    private void ChangeDay()
    {
        if (_currentDay >= 0) 
            SetDayObjectsActive(_daysObjects[_currentDay].Objects, false);
        
        if (_currentDay >= -1) 
            SetDayObjectsActive(_daysObjects[++_currentDay].Objects, true);
    }
    
    private void SetDayObjectsActive(List<GameObject> gameObjects, bool isActive)
    {
        foreach (var progressionObject in gameObjects)
            progressionObject.SetActive(isActive);
    }
    
    [Serializable]
    private class DayObjectsList
    {
        [SerializeField] private List<GameObject> _objects = new();
    
        public List<GameObject> Objects => _objects;
    }
}
