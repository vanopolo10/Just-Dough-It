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

    public void Autosave()
    {
        Save("Autosave");
    }

    public void SaveGame(string saveName)
    {
        Save(saveName);
    }

    private void Save(string saveName)
    {
        SaveSystem.CreateSave(saveName);
        SaveSystem.SaveImage(saveName);
        SaveSystem.SaveData(saveName, "CameraViewID", _cameraController.ViewID);
        SaveSystem.SaveData(saveName, "VibeLevel", Cafe.Instance.VibeLevel);
    }
}
