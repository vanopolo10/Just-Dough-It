using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private string _saveFileFolder;
    [SerializeField] private string _key;
    [SerializeField] private DoughState _state;

    [ContextMenu("Generate")]
    public void Generate()
    {
        SaveSystem.SaveData(_saveFileFolder, _key, _state);
    }

    [ContextMenu("Log")]
    public void Log()
    {
        Debug.Log(SaveSystem.LoadData<DoughState>(_saveFileFolder, _key));
        Debug.Log(DateTime.Now);
    }
}
