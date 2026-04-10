using System;
using System.Collections.Generic;
using UnityEngine;

public class DaysProgression : MonoBehaviour
{
    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private List<DayObjectsList> _daysObjects;

    private int _currentDay = -1;

    public int CurrentDay => _currentDay;

    private void OnEnable()
    {
        _customerManager.DayEnded += ChangeDay;
    }

    private void OnDisable()
    {
        _customerManager.DayEnded -= ChangeDay;
    }

    private void Awake()
    {
        foreach (var days in _daysObjects)
            SetDayObjectsActive(days.Objects, false);
    }

    private void ChangeDay()
    {
        if (_currentDay >= 0)
            SetDayObjectsActive(_daysObjects[_currentDay].Objects, false);

        if (_currentDay + 1 < _daysObjects.Count)
            SetDayObjectsActive(_daysObjects[++_currentDay].Objects, true);
    }

    private void SetDayObjectsActive(List<GameObject> gameObjects, bool isActive)
    {
        foreach (var progressionObject in gameObjects)
            if (progressionObject != null)
                progressionObject.SetActive(isActive);
    }

    public void SetDay(int day)
    {
        foreach (var days in _daysObjects)
            SetDayObjectsActive(days.Objects, false);

        _currentDay = day;

        if (_currentDay >= 0 && _currentDay < _daysObjects.Count)
            SetDayObjectsActive(_daysObjects[_currentDay].Objects, true);
    }

    [Serializable]
    private class DayObjectsList
    {
        [SerializeField] private List<GameObject> _objects = new();

        public List<GameObject> Objects => _objects;
    }
}