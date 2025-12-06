using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;

    private void Start()
    {
        int id = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "CameraViewID");
        _cameraController.SetViewID(id);

        int vibe = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "VibeLevel");
        Cafe.Instance.SetVibeLevel(vibe);
    }

    public void SaveGame()
    {
        SaveSystem.SaveImage(SaveSystem.SelectedSave);
        SaveSystem.SaveData(SaveSystem.SelectedSave, "CameraViewID", _cameraController.ViewID);
        SaveSystem.SaveData(SaveSystem.SelectedSave, "VibeLevel", Cafe.Instance.VibeLevel);
    }
}
