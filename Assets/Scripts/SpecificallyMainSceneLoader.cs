using UnityEngine;
using UnityEngine.UI;

public class SpecificallyMainSceneLoader : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.interactable = false;
    }

    public void SetInteractable(bool isInteractable)
    {
        _button.interactable = isInteractable;
    }

    public void LoadMainSceneSpecifically() 
    {
        SceneLoader.Instance.LoadScene(2);
    }
}
