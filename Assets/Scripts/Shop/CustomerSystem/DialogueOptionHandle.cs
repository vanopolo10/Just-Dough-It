using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueOptionHandle : MonoBehaviour, IPointerClickHandler
{
    private Customer _target;
    private DialogueOption _option;

    public async Task Setup(Customer target, DialogueOption option)
    {
        _target = target;
        TextMeshProUGUI optionText = GetComponentInChildren<TextMeshProUGUI>();
        
        _option = option;
        var task = LocalizationSettingsExtension.FindStringInAllTablesAsync(option.TextKey);
        await task;
        optionText.text = task.Result;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    public void PlayOut()
    {
        _target.PlayOutDialogue(_option);
        _target.DialogueManager.DeactivateDialogueOption(_option);
        Debug.Log("Played out dialogue option: " + _option.TextKey);
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