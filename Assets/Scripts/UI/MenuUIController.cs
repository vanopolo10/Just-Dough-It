using TMPro;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    public static MenuUIController Instance = null;

    [SerializeField] private TMP_InputField _saveNameInputField;
    [SerializeField] private Transform _viewportContent;
    [SerializeField] private GameObject _savePrefab;

    [SerializeField] private MenuCameraController _cameraController;

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
        if (_saveNameInputField.text == "" || SaveSystem.SaveExist(_saveNameInputField.text)) return;
        SaveSystem.CreateSave(_saveNameInputField.text);
        _saveNameInputField.text = "";
        UpdateSavesList();
        _cameraController.SetPosition(2);
    }

    public void UpdateSavesList()
    {
        for (int i = 0; i < _viewportContent.childCount; i++) Destroy(_viewportContent.GetChild(i).gameObject);
        foreach (GameSave save in SaveSystem.GetSavedGames())
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
