using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance = null;
    public Action OnLanguageChange;
    public LocalizationTable SelectedTable => _selectedTable;
    public List<LocalizationTable> Tables => _tables;
    
    [SerializeField] private List<LocalizationTable> _tables;
    private LocalizationTable _selectedTable;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance == this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        _selectedTable = _tables.FirstOrDefault();
        LoadLanguage();
        LoadTables();
    }

    public void ChangeLanguage(string code)
    {
        _selectedTable = _tables.Where(table => table.Code == code).FirstOrDefault();
        OnLanguageChange?.Invoke();

        SaveLanguage();
    }

    private void SaveLanguage()
    {
        if (_selectedTable == null) return;
        string json = JsonUtility.ToJson(new SelectedLanguage(_selectedTable.Code));
        string key = "Language.json";
        string path = Path.Combine(Application.persistentDataPath, key);

        using StreamWriter sw = new(path);
        sw.Write(json);
        sw.Close();

        Debug.Log($"[{gameObject.name}] Язык {_selectedTable.Code} сохранён");
    }

    private void LoadLanguage()
    {
        string key = "Language.json";
        string path = Path.Combine(Application.persistentDataPath, key);

        if (File.Exists(path) == false)
        {
            Debug.Log($"[{gameObject.name}] Язык не сохранялся");
            return;
        }

        using StreamReader sr = new(path);
        string json = sr.ReadToEnd();
        sr.Close();

        SelectedLanguage selectedLanguage = JsonUtility.FromJson<SelectedLanguage>(json);
        ChangeLanguage(selectedLanguage.LanguageCode);

        Debug.Log($"[{gameObject.name}] Язык {_selectedTable.Code} загружен");
    }

    public void SaveTables()
    {
        if (_tables == null) return;
        foreach (LocalizationTable table in _tables)
        {
            string json = JsonUtility.ToJson(table);
            string key = $"Table_{table.Code}.json";
            string path = Path.Combine(Application.persistentDataPath, key);

            using StreamWriter sw = new(path);
            sw.Write(json);
            sw.Close();

            Debug.Log($"[{gameObject.name}] Таблица {key} сохранена");
        }
        Debug.Log($"[{gameObject.name}] Таблицы сохранены");
    }

    public void LoadTables()
    {
        var files = Directory.GetFiles(Application.persistentDataPath, "Table_*.json", SearchOption.TopDirectoryOnly);
        if (files.Length == 0)
        {
            Debug.Log($"[{gameObject.name}] Таблицы не сохранялись");
            return;
        }

        List<LocalizationTable> tables = new() { };

        foreach (string file in files)
        {
            using StreamReader sr = new(file);
            string json = sr.ReadToEnd();
            sr.Close();

            LocalizationTable table = JsonUtility.FromJson<LocalizationTable>(json);
            tables.Add(table);
            Debug.Log($"[{gameObject.name}] Таблица {file.Split("/").Last()} загружена");
        }

        _tables = tables;

        Debug.Log($"[{gameObject.name}] Таблицы загружены");
        LoadLanguage();
    }

    public void SaveTablesIntoFiles()
    {
        if (Application.isPlaying) return;

        string json = JsonUtility.ToJson(new SerializedTables(_tables));
        string path = "Assets/Tables.json";

        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
    }

    public void LoadTablesFromFiles()
    {
        if (Application.isPlaying) return;

        string path = "Assets/Tables.json";
        if (File.Exists(path) == false)
        {
            return;
        }
        string json = File.ReadAllText(path);

        _tables = JsonUtility.FromJson<SerializedTables>(json).Tables;
    }
}

[Serializable]
public class KeyPair
{
    public string Key;
    public string Value; 
}

[Serializable]
public class LocalizationTable
{
    public string Code;
    public List<KeyPair> Keys;

    public string GetPair(string key)
    {
        return Keys.Where(pair => pair.Key == key).FirstOrDefault().Value;
    }
}

[Serializable]
public class SelectedLanguage
{
    public string LanguageCode;

    public SelectedLanguage(string code)
    {
        LanguageCode = code;
    }
}

[Serializable]
public class SerializedTables
{
    public List<LocalizationTable> Tables;
    public SerializedTables(List<LocalizationTable> tables)
    {
        Tables = tables;
    }
}