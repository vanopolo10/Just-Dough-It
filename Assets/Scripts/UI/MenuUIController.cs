using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    public static MenuUIController Instance = null;

    [SerializeField] private TMP_InputField _saveNameInputField;
    [SerializeField] private Transform _viewportContent;
    [SerializeField] private GameObject _savePrefab;

    [SerializeField] private MenuCameraController _cameraController;

    private List<GameSave> _saves;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance == this || Instance != null) Destroy(this);
    }

    private void Start()
    {
        UpdateSavesList();
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Cafe", LoadSceneMode.Single);
    }

    public void LoadLastGame()
    {
        if (_saves.Count == 0) return;
        SaveSystem.SelectedSave = _saves[0].Name;
        SceneManager.LoadScene("Cafe", LoadSceneMode.Single);
    }

    public void UpdateSavesList()
    {
        for (int i = 0; i < _viewportContent.childCount; i++) Destroy(_viewportContent.GetChild(i).gameObject);
        _saves = SaveSystem.GetSavedGames().OrderByDescending(s => DateTime.Parse(s.ChangeTime)).ToList();
        foreach (GameSave save in _saves)
        {
            GameObject saveUIElement = Instantiate(_savePrefab, _viewportContent);
            saveUIElement.GetComponent<SaveUI>().ChangeInfo(save.Name, save.ChangeTime, SaveSystem.LoadSprite(save.Name));
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
