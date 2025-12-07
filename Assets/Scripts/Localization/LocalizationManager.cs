using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEditor.AssetDatabase;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance = null;

    [SerializeField] private List<LocalizationTable> _tables;

    public event Action OnLanguageChange;

    public LocalizationTable SelectedTable { get; private set; }

    public List<LocalizationTable> Tables => _tables;

    private readonly JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance == this || Instance != null) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        SelectedTable = _tables.FirstOrDefault();
        LoadLanguage();
        LoadTables();
    }

    public void ChangeLanguage(string code)
    {
        SelectedTable = _tables.FirstOrDefault(table => table.Code == code);
        OnLanguageChange?.Invoke();

        SaveLanguage();
    }

    private void SaveLanguage()
    {
        if (SelectedTable == null) return;
        string json = JsonConvert.SerializeObject(new SelectedLanguage(SelectedTable.Code));
        string key = "Language.json";
        string path = Path.Combine(Application.persistentDataPath, key);

        File.WriteAllText(path, json);

        Debug.Log($"[{gameObject.name}] Язык {SelectedTable.Code} сохранён");
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

        string json = File.ReadAllText(path);

        SelectedLanguage selectedLanguage = JsonConvert.DeserializeObject<SelectedLanguage>(json, _settings);
        ChangeLanguage(selectedLanguage.LanguageCode);

        Debug.Log($"[{gameObject.name}] Язык {SelectedTable.Code} загружен");
    }

    public void SaveTables()
    {
        if (_tables == null) return;
        foreach (LocalizationTable table in _tables)
        {
            string json = JsonConvert.SerializeObject(table, _settings);
            string key = $"Table_{table.Code}.json";
            string path = Path.Combine(Application.persistentDataPath, key);

            File.WriteAllText(path, json);

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
            string json = File.ReadAllText(file);

            LocalizationTable table = JsonConvert.DeserializeObject<LocalizationTable>(json, _settings);
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

        string json = JsonConvert.SerializeObject(_tables, _settings);
        string path = "Assets/Tables.json";

        File.WriteAllText(path, json);
        Refresh();
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

        _tables = JsonConvert.DeserializeObject<List<LocalizationTable>>(json, _settings);
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
    public string GetPair(string key)
    {
        return Keys.FirstOrDefault(pair => pair.Key == key)?.Value;
    }

    public string Code { get; set; }
    public List<KeyPair> Keys { get; set; }
}

[Serializable]
public class SelectedLanguage
{
    public SelectedLanguage(string code)
    {
        LanguageCode = code;
    }

    public string LanguageCode { get; }
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