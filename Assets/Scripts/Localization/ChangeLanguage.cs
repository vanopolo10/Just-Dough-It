using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Linq;

public class ChangeLanguage : MonoBehaviour
{

    public void ChangeLanguageByCode(string languageCode)
    {
        StartCoroutine(SetLocale(languageCode));
    }

    public static IEnumerator SetLocale(string languageCode)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Where(loc => loc.Identifier.Code == languageCode).LastOrDefault();
        Debug.Log("[ChangeLanguage] язык успешно изменЄн");
        yield break;
    }
}
