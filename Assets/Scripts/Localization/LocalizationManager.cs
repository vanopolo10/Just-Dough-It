using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class KeyPair
{
    public string Key;
    public string Value;
}

[Serializable]
public class LocalizationTable
{
    public string Code; // "ru", "en"
    public List<KeyPair> Keys = new List<KeyPair>();

    public string GetPair(string key)
    {
        if (Keys == null || Keys.Count == 0 || string.IsNullOrEmpty(key))
            return key;

        for (int i = 0; i < Keys.Count; i++)
        {
            if (Keys[i].Key == key)
                return Keys[i].Value;
        }

        return key;
    }
}

[Serializable]
public class LocalizationTableFile
{
    public string Code;
    public List<KeyPair> Keys = new List<KeyPair>();
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- ДАННЫЕ ЛОКАЛИЗАЦИИ В РЕДАКТОРЕ И РАНТАЙМЕ ----------

    [Tooltip("Все языковые таблицы. В ЭДИТОРЕ всё сохраняется только сюда.")]
    public List<LocalizationTable> Tables = new List<LocalizationTable>();

    [Tooltip("Текущая активная таблица (выбранный язык).")]
    public LocalizationTable SelectedTable;

    public event Action OnLanguageChange;

    public string GetLocalized(string key)
    {
        if (SelectedTable == null)
            return key;

        return SelectedTable.GetPair(key);
    }

    public void SetLanguage(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("[Localization] Empty language code.");
            return;
        }

        if (Tables == null || Tables.Count == 0)
        {
            Debug.LogWarning("[Localization] Tables list is empty, cannot set language: " + code);
            return;
        }

        LocalizationTable table = null;

        for (int i = 0; i < Tables.Count; i++)
        {
            if (Tables[i].Code == code)
            {
                table = Tables[i];
                break;
            }
        }

        if (table == null)
        {
            Debug.LogWarning("[Localization] Table not found for code: " + code);
            return;
        }

        SelectedTable = table;
        OnLanguageChange?.Invoke();
        Debug.Log("[Localization] Language set to: " + code);
    }

    public void ChangeLanguage(string code)
    {
        SetLanguage(code);
    }

    // ---------- РАБОТА С JSON ФАЙЛАМИ В ПРОЕКТЕ (Только редактор) ----------

#if UNITY_EDITOR
    private static string ProjectLocalizationFolder =>
        Path.Combine(Application.dataPath, "StreamingAssets/Localization");

    [ContextMenu("Save Tables Into Files (JSON)")]
    public void SaveTablesIntoFiles()
    {
        if (Tables == null || Tables.Count == 0)
        {
            Debug.LogWarning("[Localization] No tables to save.");
            return;
        }

        Directory.CreateDirectory(ProjectLocalizationFolder);

        foreach (var table in Tables)
        {
            if (table == null || string.IsNullOrEmpty(table.Code))
            {
                Debug.LogWarning("[Localization] Table with empty or null code skipped.");
                continue;
            }

            var fileData = new LocalizationTableFile
            {
                Code = table.Code,
                Keys = new List<KeyPair>(table.Keys ?? new List<KeyPair>())
            };

            string json = JsonUtility.ToJson(fileData, true);
            string filePath = Path.Combine(ProjectLocalizationFolder, table.Code + ".json");

            File.WriteAllText(filePath, json, Encoding.UTF8);
            Debug.Log("[Localization] Saved: " + filePath);
        }

        AssetDatabase.Refresh();
    }

    [ContextMenu("Load Tables From Files (JSON)")]
    public void LoadTablesFromFiles()
    {
        if (!Directory.Exists(ProjectLocalizationFolder))
        {
            Debug.LogWarning("[Localization] Folder not found: " + ProjectLocalizationFolder);
            return;
        }

        Tables ??= new List<LocalizationTable>();
        Tables.Clear();

        foreach (var filePath in Directory.GetFiles(ProjectLocalizationFolder, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                var fileData = JsonUtility.FromJson<LocalizationTableFile>(json);

                var table = new LocalizationTable
                {
                    Code = fileData.Code,
                    Keys = new List<KeyPair>(fileData.Keys ?? new List<KeyPair>())
                };

                Tables.Add(table);

                Debug.Log("[Localization] Loaded: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("[Localization] Error reading " + filePath + ":\n" + e);
            }
        }

        EditorUtility.SetDirty(this);
    }
#endif

    // ---------- ЧТЕНИЕ JSON В БИЛДЕ (StreamingAssets) ----------

    public void LoadTablesFromStreamingAssets()
    {
        Tables ??= new List<LocalizationTable>();
        Tables.Clear();

        string folder = Path.Combine(Application.streamingAssetsPath, "Localization");

        if (!Directory.Exists(folder))
        {
            Debug.LogWarning("[Localization] StreamingAssets folder not found: " + folder);
            return;
        }

        foreach (var filePath in Directory.GetFiles(folder, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                var fileData = JsonUtility.FromJson<LocalizationTableFile>(json);

                var table = new LocalizationTable
                {
                    Code = fileData.Code,
                    Keys = new List<KeyPair>(fileData.Keys ?? new List<KeyPair>())
                };

                Tables.Add(table);
            }
            catch (Exception e)
            {
                Debug.LogError("[Localization] Error reading " + filePath + ":\n" + e);
            }
        }

        Debug.Log("[Localization] Loaded " + Tables.Count + " tables from StreamingAssets.");
    }
}
