using UnityEngine;

public class LocalizationBootstrap : MonoBehaviour
{
    [SerializeField] private LocalizationManager _manager;

    private void Awake()
    {
        _manager.LoadTablesFromStreamingAssets();

        string langCode = Application.systemLanguage switch
        {
            SystemLanguage.Russian => "ru",
            SystemLanguage.English => "en",
            _ => "en"
        };

        _manager.SetLanguage(langCode);
    }
}