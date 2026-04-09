using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionTracker : MonoBehaviour
{
    private double _startTime;
    private double _endTime;

    private int _sessionId;

    private void OnEnable()
    {
        if (SaveSystem.TryLoadData<List<double>>(SaveSystem.SelectedSave, "Sessions", out List<double> sessions))
            _sessionId = sessions.Count;
        else
            _sessionId = 0;

        _startTime = Time.realtimeSinceStartup;

        StartCoroutine(Counter());

        SceneManager.sceneUnloaded += Save;
    }

    private void OnDisable()
    {
        Save();
        SceneManager.sceneUnloaded -= Save;
    }

    private void Save(Scene scene) => Save();

    private void Save()
    {
        _endTime = Time.realtimeSinceStartupAsDouble;
        StopAllCoroutines();
        SaveSession();
    }

    private IEnumerator Counter()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            _endTime = Time.realtimeSinceStartupAsDouble;
            SaveSession();
        }
    }

    private void SaveSession()
    {
        string currentSave = SaveSystem.SelectedSave;

        if (SaveSystem.TryLoadData<List<double>>(currentSave, "Sessions", out List<double> sessions))
        {
            if (_sessionId < sessions.Count && _sessionId > 0)
                sessions[_sessionId] = _endTime - _startTime;
            else
                sessions.Add(_endTime - _startTime);
            SaveSystem.SaveData(currentSave, "Sessions", sessions);
        }
        else
        {
            if (!SaveSystem.SaveExist(currentSave))
                SaveSystem.CreateSave(currentSave);
            SaveSystem.SaveData(currentSave, "Sessions", new List<double>());
            SaveSession();
        }
            
    }
}
