#if UNITY_EDITOR
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class LocalizationExporter
{
    [MenuItem("Localization/Export Tables To Build")]
    public static void ExportTablesToBuild()
    {
        LocalizationManager manager = Object.FindFirstObjectByType<LocalizationManager>();
        if (manager == null)
        {
            Debug.LogError("[LocalizationExporter] На сцене нет LocalizationManager");
            return;
        }

        var tables = manager.Tables;
        
        if (tables == null || tables.Count == 0)
        {
            Debug.LogWarning("[LocalizationExporter] Нет таблиц для экспорта");
            return;
        }

        string dir = Path.Combine(Application.dataPath, "StreamingAssets/Localization");
        Directory.CreateDirectory(dir);

        JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        foreach (var table in tables)
        {
            if (string.IsNullOrEmpty(table.Code))
            {
                Debug.LogWarning("[LocalizationExporter] Таблица без кода, пропускаю");
                continue;
            }

            string json = JsonConvert.SerializeObject(table, settings);
            string filePath = Path.Combine(dir, $"Table_{table.Code}.json");
            File.WriteAllText(filePath, json);
            Debug.Log($"[LocalizationExporter] Таблица {table.Code} сохранена в {filePath}");
        }

        AssetDatabase.Refresh();
    }
}
#endif