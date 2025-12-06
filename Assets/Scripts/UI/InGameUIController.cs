using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private GameObject _ui;

    private void Start()
    {
        _ui.SetActive(false);
        _cameraController.enabled = true;
    }

    private void Switch()
    {
        _ui.SetActive(!_ui.activeSelf);
        _cameraController.enabled = !_cameraController.enabled;
    }

    private void OnEscape()
    {
        Switch();
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void ChangeLanguage(string code)
    {
        LocalizationManager.Instance.ChangeLanguage(code);
    }
}
