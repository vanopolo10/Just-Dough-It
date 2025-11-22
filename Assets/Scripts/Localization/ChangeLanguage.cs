using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChangeLanguage : MonoBehaviour
{

    public void ChangeLanguageByID(int LocaleID)
    {
        StartCoroutine(SetLocale(LocaleID));
    }

    public static IEnumerator SetLocale(int LocaleID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LocaleID];
        Debug.Log("[ChangeLanguage] язык успешно изменЄн");
        yield break;
    }
}
