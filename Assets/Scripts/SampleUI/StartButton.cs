using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void SwitchSceneToGame()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
