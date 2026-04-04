using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private static readonly int TrayClose = Animator.StringToHash("TrayClose");
    private static readonly int ShelfClose = Animator.StringToHash("ShelfClose");
    private static readonly int Close = Animator.StringToHash("Close");

    [SerializeField] private GameObject _choice;
    [SerializeField] private GameObject _cafeExsist;
    [SerializeField] private GameObject _cafeName;
    [SerializeField] private CafeNameController _cafeNameController;

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
        _choice.SetActive(false);
        _cafeExsist.SetActive(false);
        _cafeName.SetActive(true);
        _cafeNameController.Clear();


        _sides.SetTrigger(TrayClose);
        _sides.SetTrigger(ShelfClose);
        
        _newGame.SetTrigger(Close);
        
        _settings.interactable = true;
        _load.interactable = true;
    }
}
