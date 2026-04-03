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
    [SerializeField] private GameObject _saveUI;

    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Dropdown _dropdown;

    private Book _book;

    public bool IsMenuOpen => _ui.activeSelf || _saveUI.activeSelf;

    private void Awake()
    {
        _book = GetComponent<Book>();
    }

    private void Start()
    {
        _ui.SetActive(false);
        _saveUI.SetActive(false);
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

        if (_saveUI.activeSelf)
            SwitchSaveMenu();
        else
            Switch();
    }

    public void SwitchSaveMenu()
    {
        _saveUI.SetActive(!_saveUI.activeSelf);
        _ui.SetActive(!_ui.activeSelf);
    }

    public void ExitButton()
    {
        _saveManager.SaveGame();
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