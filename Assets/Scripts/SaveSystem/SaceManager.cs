using UnityEngine;

public class SaceManager : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;

    private void Start()
    {
        Debug.Log(SaveSystem.SelectedSave);

        int id = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "CameraViewID");
        Debug.Log(id);
        _cameraController.SetViewID(id);

        int vibe = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "VibeLevel");
        Debug.Log(vibe);
        Cafe.Instance.SetVibeLevel(vibe);
    }

    [ContextMenu("Force Save")]
    public void SaveGame()
    {
        SaveSystem.SaveImage(SaveSystem.SelectedSave);
        SaveSystem.SaveData(SaveSystem.SelectedSave, "CameraViewID", _cameraController.ViewID);
        SaveSystem.SaveData(SaveSystem.SelectedSave, "VibeLevel", Cafe.Instance.VibeLevel);
    }
}
