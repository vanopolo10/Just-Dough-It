using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject _saveUI;

    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private TMP_Dropdown _dropdown;
    
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

    public void CreateSaveButton()
    {
        if (_inputField.text == "") return;
        
        _saveManager.SaveGame(_inputField.text);
        _inputField.text = "";
        _audioSource.Play();
        SwitchSaveMenu();
    }

    public void ExitButton()
    {
        _saveManager.Autosave();
        _audioSource.Play();
        Invoke(nameof(Exit), 1);
    }

    private void Exit()
    {
        SceneManager.LoadScene("MainMenu-draft", LoadSceneMode.Single);
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
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(code));
        SaveSystem.SaveCurrentLanguage();
        yield return LocalizationSettings.InitializationOperation;
    }
}
