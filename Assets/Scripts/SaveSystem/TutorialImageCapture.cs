using UnityEngine;

public class TutorialImageCapture : MonoBehaviour
{
    private void Start()
    {
        Invoke("Screenshot", 2f);
    }
    public void Screenshot()
    {
        SaveSystem.SaveImage(SaveSystem.SelectedSave);
    }
}
