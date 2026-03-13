using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private static readonly int TrayClose = Animator.StringToHash("TrayClose");
    private static readonly int ShelfClose = Animator.StringToHash("ShelfClose");
    private static readonly int Close = Animator.StringToHash("Close");

    [SerializeField] private Button _settings;
    [SerializeField] private Button _load;
    
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
        _sides.SetTrigger(TrayClose);
        _sides.SetTrigger(ShelfClose);
        
        _newGame.SetTrigger(Close);
        
        _settings.interactable = true;
        _load.interactable = true;
    }
}
