using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void ChangeSceneToMain()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
