using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public static MenuUIController Instance = null;

    [SerializeField] private CafeNameController _cafeNameController;
    [SerializeField] private Transform _viewportContent;
    [SerializeField] private GameObject _savePrefab;
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private Animator _sidesAnimator;
    [SerializeField] private Button _continueButton;

    [Header("Cafe Name, Sunrizes & Tutorial")]
    [SerializeField] private GameObject _choice;
    [SerializeField] private GameObject _cafeExsist;
    [SerializeField] private GameObject _cafeName;
    [SerializeField] private GameObject _tutorial;

    private List<GameSave> _saves;
    private List<string> _languageCodes = new() { "ru", "en" };
    private bool _doPreferSunrises;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        UpdateSavesList();
        StartCoroutine(SetLanguage(SaveSystem.GetSaveLanguage()));
        _choice.SetActive(false);
        _cafeExsist.SetActive(false);
        _tutorial.SetActive(false);
        _cafeName.SetActive(true);
    }

    public void NewGame(bool needTutorial)
    {
        SaveSystem.SelectedSave = _cafeNameController.CafeName;
        SaveSystem.CreateSave(SaveSystem.SelectedSave);
        SaveSystem.SaveData(SaveSystem.SelectedSave, "DoPreferSunrises", _doPreferSunrises);

        SceneLoader.Instance.LoadScene(needTutorial ? 1 : 2);
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

        _continueButton.interactable = _saves.Count != 0;
        
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

    public void CafeNameSubmit()
    {
        if (SaveSystem.SaveExist(_cafeNameController.CafeName))
        {
            _cafeName.SetActive(false);
            _cafeExsist.SetActive(true);
        }
        else
        {
            _cafeName.SetActive(false);
            _choice.SetActive(true);
        }
    }

    public void RewriteSave()
    {
        SaveSystem.DeleteSave(_cafeNameController.CafeName);
        _cafeExsist.SetActive(false);
        _choice.SetActive(true);
    }

    public void SetPreferSunrises(bool preferSunrises)
    {
        _doPreferSunrises = preferSunrises;
        _choice.SetActive(false);
        _tutorial.SetActive(true);
    }

    private IEnumerator SetLanguage(string code)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(code));
        SaveSystem.SaveCurrentLanguage();
    }
}