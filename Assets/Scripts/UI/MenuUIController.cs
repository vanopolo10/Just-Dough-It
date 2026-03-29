using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class MenuUIController : MonoBehaviour
{
    public static MenuUIController Instance = null;

    [SerializeField] private CafeNameController _cafeNameController;
    [SerializeField] private Transform _viewportContent;
    [SerializeField] private GameObject _savePrefab;
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private Animator _sidesAnimator;

    private List<GameSave> _saves;
    private List<string> _languageCodes = new() { "ru", "en" };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        UpdateSavesList();
        StartCoroutine(SetLanguage(SaveSystem.GetSaveLanguage()));
    }

    public void NewGame(bool doPreferSunrises)
    {
        SaveSystem.SelectedSave = _cafeNameController.CafeName;
        SaveSystem.CreateSave(SaveSystem.SelectedSave);
        SaveSystem.SaveData(SaveSystem.SelectedSave, "DoPreferSunrises", doPreferSunrises);

        SceneLoader.Instance.LoadScene(1);
    }

    public void LoadLastGame()
    {
        if (_saves.Count == 0) return;

        SaveSystem.SelectedSave = _saves[0].Name;
        SceneLoader.Instance.LoadScene(2);
    }

    public void UpdateSavesList()
    {
        for (int i = 0; i < _viewportContent.childCount; i++)
            Destroy(_viewportContent.GetChild(i).gameObject);

        _saves = SaveSystem.GetSavedGames()
            .OrderByDescending(s => System.DateTime.Parse(s.ChangeTime))
            .ToList();

        foreach (GameSave save in _saves)
        {
            GameObject saveUIElement = Instantiate(_savePrefab, _viewportContent);
            SaveUI saveUI = saveUIElement.GetComponent<SaveUI>();
            saveUI.ChangeInfo(save.Name, save.ChangeTime, SaveSystem.LoadSprite(save.Name));
            saveUI.SetAnimator(_sidesAnimator);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeLanguage(int _)
    {
        StartCoroutine(SetLanguage(_languageCodes[_languageDropdown.value]));
    }

    private IEnumerator SetLanguage(string code)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(code));
        SaveSystem.SaveCurrentLanguage();
    }
}