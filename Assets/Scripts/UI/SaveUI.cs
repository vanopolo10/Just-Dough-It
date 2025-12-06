using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _createTime;
    [SerializeField] private Image _image;

    [SerializeField] private GameObject _mainUI;
    [SerializeField] private GameObject _deleteUI;

    public void ChangeInfo(string name, string createTime, Sprite thumbnail)
    {
        _name.text = name;
        _createTime.text = createTime;
        _image.sprite = thumbnail;

        _mainUI.SetActive(true);
        _deleteUI.SetActive(false);
    }

    public void StartGame()
    {
        SaveSystem.SelectedSave = _name.text;
        SceneManager.LoadScene("Cafe", LoadSceneMode.Single);
    }

    public void RemoveSave()
    {
        SaveSystem.DeleteSave(_name.text);
        MenuUIController.Instance.UpdateSavesList();
    }

    public void SwitchDeleteUI()
    {
        _mainUI.SetActive(!_mainUI.activeSelf);
        _deleteUI.SetActive(!_deleteUI.activeSelf);
    }
}
