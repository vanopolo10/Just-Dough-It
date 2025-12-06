using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _createTime;
    [SerializeField] private Image _image;

    public void ChangeInfo(string name, string createTime, Sprite thumbnail)
    {
        _name.text = name;
        _createTime.text = createTime;
        _image.sprite = thumbnail;
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
}
