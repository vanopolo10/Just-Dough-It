using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private static readonly int TrayClose = Animator.StringToHash("TrayClose");
    private static readonly int ShelfClose = Animator.StringToHash("ShelfClose");
    private static readonly int Close = Animator.StringToHash("Close");

    [SerializeField] private GameObject _choices;
    [SerializeField] private GameObject _cafeName;
    
    [Header("Buttons")]
    [SerializeField] private Button _settings;
    [SerializeField] private Button _load;
    
    [Header("Animators")]
    [SerializeField] private Animator _sides;
    [SerializeField] private Animator _newGame;

    public void DisableButtonsAndWindows()
    {
        _settings.interactable = false;
        _load.interactable = false;
        
        _sides.SetTrigger(TrayClose);
        _sides.SetTrigger(ShelfClose);
    }
    
    private void OnEscape()
    {
        if (_choices.activeSelf)
        {
            _choices.SetActive(false);
            _cafeName.SetActive(true);
            return;
        }
        
        _sides.SetTrigger(TrayClose);
        _sides.SetTrigger(ShelfClose);
        
        _newGame.SetTrigger(Close);
        
        _settings.interactable = true;
        _load.interactable = true;
    }
}
