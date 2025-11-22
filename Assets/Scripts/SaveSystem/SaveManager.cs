using System;
using System.IO;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SaveManager : MonoBehaviour
{
    public void Start()
    {
        LoadLanguage();
    }

    public void SaveLanguage()
    {
        SelectedLanguage selectedLanguage = new(LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale));
        string json = JsonUtility.ToJson(selectedLanguage);
        string key = "Language.json";
        string path = Path.Combine(Application.persistentDataPath, key);

        using StreamWriter streamWriter = new(path);
        streamWriter.Write(json);

        Debug.Log("[SaveManager] Выбранный язык успешно сохранён");
    }

    public void LoadLanguage()
    {
        string key = "Language.json";
        string path = Path.Combine(Application.persistentDataPath, key);

        if (File.Exists(path) is false)
        {
            Debug.Log("[SaveManager] Сохранения языка не было");
            return;
        }

        using StreamReader streamReader = new(path);
        string json = streamReader.ReadToEnd();

        SelectedLanguage selectedLanguange = JsonUtility.FromJson<SelectedLanguage>(json);
        StartCoroutine(ChangeLanguage.SetLocale(selectedLanguange.LanguageID));

        Debug.Log("[SaveManager] Сохранённый язык успешно загружен");
    }
}

[Serializable]
public class SelectedLanguage
{
    public int LanguageID;

    public SelectedLanguage(int ID)
    {
        this.LanguageID = ID;
    }
}