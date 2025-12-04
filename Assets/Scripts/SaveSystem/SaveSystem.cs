using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveSystem
{
    public static string SelectedSave = "None";

    private static JsonSerializerSettings _settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented
    };

    public static void SaveData(string saveFileName, string key, object value)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Saves", $"{saveFileName}\\");
        string path = Path.Combine(directoryPath, "save.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameSave gameState = JsonConvert.DeserializeObject<GameSave>(json, _settings);
            SaveableValue saveableValue = gameState.SavedData.FirstOrDefault(data => data.Key == key);
            if (saveableValue != null)
            {
                int id = gameState.SavedData.IndexOf(saveableValue);
                gameState.SavedData[id] = new(key, value);
            }
            else
            {
                gameState.SavedData.Add(new(key, value));
            }
            gameState.ChangeTime = DateTime.Now.ToString();
            json = JsonConvert.SerializeObject(gameState, _settings);
            File.WriteAllText(path, json);
        }
        else return;
    }

    public static T LoadData<T>(string saveFileName, string key)
    {
        string path = Path.Combine(Application.persistentDataPath, "Saves", $"{saveFileName}\\", "save.json");

        if (DataExist(saveFileName, key) == false) return default;
        string json = File.ReadAllText(path);
        GameSave gameState = JsonConvert.DeserializeObject<GameSave>(json, _settings);
        var value = gameState.SavedData.First(data => data.Key == key).Value;
        if (value is T typedValue)
            return typedValue;
        return default;
    }

    public static bool DataExist(string saveFileName, string key)
    {
        string path = Path.Combine(Application.persistentDataPath, "Saves", $"{saveFileName}\\", "save.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameSave gameState = JsonConvert.DeserializeObject<GameSave>(json, _settings);
            if (gameState.SavedData.First(data => data.Key == key) != null) return true;
        }
        return false;
    }

    public static void SaveImage(string saveFileName)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Saves", $"{saveFileName}\\");
        string path = Path.Combine(directoryPath, "thumbnail.png");
        if (File.Exists(path))
            ScreenCapture.CaptureScreenshot(path);
        else return;
    }

    public static Sprite LoadSprite(string saveFileName)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Saves", $"{saveFileName}\\");
        string path = Path.Combine(directoryPath, "thumbnail.png");
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            
            Texture2D texture = new(1, 1);
            texture.LoadImage(bytes);

            Sprite sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(0.5f, 0.5f));
            return sprite;
        }
        else return null;
    }

    public static List<GameSave> GetSavedGames()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Saves");
        List<GameSave> savedGames = new();

        if (Directory.Exists(directoryPath) == false) return savedGames;
        foreach (string directory in Directory.GetDirectories(directoryPath))
        {
            string path = Path.Combine(directory, "save.json");
            if (File.Exists(path) == false) continue;

            string json = File.ReadAllText(path);
            GameSave save = JsonConvert.DeserializeObject<GameSave>(json);
            savedGames.Add(save);
        }

        return savedGames;
    }

    public static void CreateSave(string saveFileName)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Saves", $"{saveFileName}\\");
        string path = Path.Combine(directoryPath, "save.json");

        if (Directory.Exists(directoryPath) == true)
            DeleteSave(saveFileName);

        Directory.CreateDirectory(directoryPath);
        GameSave gameState = new(saveFileName);
        gameState.ChangeTime = DateTime.Now.ToString();
        string json = JsonConvert.SerializeObject(gameState, _settings);
        File.WriteAllText(path, json);
    }

    public static void DeleteSave(string saveFileName)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Saves", $"{saveFileName}\\");

        if (Directory.Exists(directoryPath) == false)
            return;

        foreach (string file in Directory.GetFiles(directoryPath))
        {
            File.Delete(file);
        }
        Directory.Delete(directoryPath);
    }

    public static bool SaveExist(string saveFileName)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Saves", $"{saveFileName}\\");
        string path = Path.Combine(directoryPath, "save.json");

        if (File.Exists(path) && Directory.Exists(directoryPath)) return true;
        return false;
    }
}

[Serializable]
public class SaveableValue
{
    public string Key;
    public object Value;

    public SaveableValue(string key, object value)
    {
        Key = key;
        Value = value;
    }
}

[Serializable]
public class GameSave
{
    public string Name;
    public string ChangeTime;
    public List<SaveableValue> SavedData;

    public GameSave(string saveName)
    {
        Name = saveName;
        ChangeTime = DateTime.Now.ToString();
        SavedData = new();
    }
}