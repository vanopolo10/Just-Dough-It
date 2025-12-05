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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Switch();
        }
    }

    private void Switch()
    {
        _ui.SetActive(!_ui.activeSelf);
        _cameraController.enabled = !_cameraController.enabled;
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("SampleUI", LoadSceneMode.Single);
    }
}
