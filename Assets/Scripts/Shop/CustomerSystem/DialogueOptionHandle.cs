using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueOptionHandle : MonoBehaviour, IPointerClickHandler
{
    private Customer _target;
    private DialogueOption _option;
    public void Setup(Customer target, DialogueOption option)
    {
        _target = target;
        TextMeshProUGUI optionText = GetComponentInChildren<TextMeshProUGUI>();
        _option = option;
        optionText.text = option.textKey; // replace with localization key
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
    public void PlayOut()
    {
        _target.PlayOutDialogue(_option);
        _target.DialogueManager.DeactivateDialogueOption(_option);
        Debug.Log("Played out dialogue option: " + _option.textKey);
    }
    public void OnMouseDown()
    {
        PlayOut();
    }
    public void OnPointerClick(PointerEventData ignored)
    {
        PlayOut();
    }
}
    

    


