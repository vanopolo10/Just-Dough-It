using UnityEngine;

public class SettingsToMain : MonoBehaviour
{
    [SerializeField] private GameObject _main;
    [SerializeField] private GameObject _settings;

    private void Start()
    {
        ToMain();
    }

    public void ToMain()
    {
        _main.SetActive(true);
        _settings.SetActive(false);
    }

    public void ToSettings()
    {
        _main.SetActive(false);
        _settings.SetActive(true);
    }
}
