using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    public static MenuUIController Instance = null;

    [SerializeField] private Darkness _darkness;
    [SerializeField] private TMP_InputField _saveNameInputField;
    [SerializeField] private Transform _viewportContent;
    [SerializeField] private GameObject _savePrefab;
    
    private List<GameSave> _saves;
    private bool _doPreferSunrises;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance == this || Instance != null) Destroy(this);

        _darkness.Darkened += LoadNewGame;
    }

    private void Start()
    {
        UpdateSavesList();
        StartCoroutine(SetLanguage(SaveSystem.GetSaveLanguage()));
    }

    public void NewGame(bool doPreferSunrises)
    {
        _darkness.FallAsleep();
        _doPreferSunrises = doPreferSunrises;
    }

    private void LoadNewGame()
    {
        SceneManager.LoadScene("CustomerIntegration", LoadSceneMode.Single);
    }

    public void LoadLastGame()
    {
        if (_saves.Count == 0) return;
        SaveSystem.SelectedSave = _saves[0].Name;
        SceneManager.LoadScene("CustomerIntegration", LoadSceneMode.Single);
    }

    public void UpdateSavesList()
    {
        for (int i = 0; i < _viewportContent.childCount; i++) 
            Destroy(_viewportContent.GetChild(i).gameObject);
        
        _saves = SaveSystem.GetSavedGames().OrderByDescending(s => DateTime.Parse(s.ChangeTime)).ToList();
        
        foreach (GameSave save in _saves)
        {
            GameObject saveUIElement = Instantiate(_savePrefab, _viewportContent);
            saveUIElement.GetComponent<SaveUI>().ChangeInfo(save.Name, save.ChangeTime, SaveSystem.LoadSprite(save.Name));
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadLanguage(string code)
    {
        StartCoroutine(SetLanguage(code));
    }

    private IEnumerator SetLanguage(string code)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(code));
        SaveSystem.SaveCurrentLanguage();
    }
}
