using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _ui;

    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private TMP_Dropdown _dropdown;

    private Book _book;

    public bool IsMenuOpen => _ui.activeSelf;

    private void Awake()
    {
        _book = GetComponent<Book>();
    }

    private void Start()
    {
        _ui.SetActive(false);
        _cameraController.enabled = true;

        StartCoroutine(SetLanguage(SaveSystem.GetSaveLanguage()));
    }

    private void Switch()
    {
        _ui.SetActive(!_ui.activeSelf);
        _cameraController.enabled = !_cameraController.enabled;
    }

    private void OnEscape()
    {
        if (_book.IsOpen)
            return;
        
        _dropdown.value = _dropdown.options.FindIndex(option => option.text == LocalizationSettings.SelectedLocale.LocaleName);

        Switch();
    }

    public void Save()
    {
        _saveManager.SaveGame();
    }

    public void Exit()
    {
        Save();
        SceneLoader.Instance.LoadScene(0);
    }

    public void ChangeLanguage()
    {
        string code = _dropdown.options[_dropdown.value].text.ToLower();

        switch (code)
        {
            case "english":
                StartCoroutine(SetLanguage("en"));
                break;
            case "русский":
                StartCoroutine(SetLanguage("ru"));
                break;
        }
    }

    private IEnumerator SetLanguage(string code)
    {
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(code));

        SaveSystem.SaveCurrentLanguage();

        yield return LocalizationSettings.InitializationOperation;
    }
}