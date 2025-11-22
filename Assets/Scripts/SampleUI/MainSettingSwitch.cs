using UnityEngine;

public class MainSettingSwitch : MonoBehaviour
{
    [SerializeField] private GameObject _main;
    [SerializeField] private GameObject _settings;

    public void Start()
    {
        SwitchToMain();
    }

    public void SwitchToMain()
    {
        _main.SetActive(true);
        _settings.SetActive(false);
    }

    public void SwitchToSettings()
    {
        _main.SetActive(false);
        _settings.SetActive(true);
    }
}
