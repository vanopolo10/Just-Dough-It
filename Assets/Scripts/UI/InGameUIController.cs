using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject _saveUI;

    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private TMP_InputField _inputField;

    private void Start()
    {
        _ui.SetActive(false);
        _saveUI.SetActive(false);
        _cameraController.enabled = true;
    }

    private void Switch()
    {
        _ui.SetActive(!_ui.activeSelf);
        _cameraController.enabled = !_cameraController.enabled;
    }

    private void OnEscape()
    {
        if (_saveUI.activeSelf)
            SwitchSaveMenu();
        else Switch();
    }

    public void SwitchSaveMenu()
    {
        _saveUI.SetActive(!_saveUI.activeSelf);
        _ui.SetActive(!_ui.activeSelf);
    }

    public void CreateSaveButton()
    {
        if (_inputField.text != "")
        {
            _saveManager.SaveGame(_inputField.text);
            _inputField.text = "";
            SwitchSaveMenu();
        }
    }

    public void ExitButton()
    {
        _saveManager.Autosave();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void ChangeLanguage(string code)
    {
        LocalizationManager.Instance.ChangeLanguage(code);
    }
}
