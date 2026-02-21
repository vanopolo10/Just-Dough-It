using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomerInteraction
{
    [SerializeField] private string _dialogueKey;
    //[SerializeField] private string _triggerName;
    
    public string DialogueKey => _dialogueKey;

    public void PlayOut(Customer target)
    {
        // target.TryGetComponent(out Animator animator);
        //
        // if (animator)
        //     animator.SetTrigger(_triggerName);
        
        if (target == null)
        {
            Debug.LogError("Target customer is null");
            return;
        }

        DialogueManager dialogueManager = target.DialogueManager;
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager is null on target customer");
            return;
        }

        dialogueManager.DisplayText(_dialogueKey);
    }
}

[Serializable]
public struct DialogueOption
{
    [SerializeField] private string _textKey;
    [SerializeField] private CustomerInteraction _interaction;
    
    public string TextKey => _textKey;
    public CustomerInteraction Interaction => _interaction;
    
    public DialogueOption(string textKey, CustomerInteraction interaction)
    {
        _textKey = textKey;
        _interaction = interaction;
    }
}

[CreateAssetMenu(fileName = "CustomerInteraction",
    menuName = "ScriptableObjects/CustomerSystem/CustomerInteractionSet")]
public class CustomerInteractionSet : ScriptableObject
{
    [SerializeField] private CustomerInteraction _onGreeting;
    [SerializeField] private CustomerInteraction _onItemAccepted;
    [SerializeField] private CustomerInteraction _onItemRejected;
    [SerializeField] private CustomerInteraction _onQuestCompleted;
    [SerializeField] private List<DialogueOption> _dialogueOptions = new();
    
    public CustomerInteraction OnGreeting => _onGreeting;
    public CustomerInteraction OnItemAccepted => _onItemAccepted;
    public CustomerInteraction OnItemRejected => _onItemRejected;
    public CustomerInteraction OnQuestCompleted => _onQuestCompleted;
    public List<DialogueOption> DialogueOptions => _dialogueOptions;
    
    public void AddDialogueOption(DialogueOption option)
    {
        _dialogueOptions.Add(option);
    }
    
    public void RemoveDialogueOption(DialogueOption option)
    {
        _dialogueOptions.Remove(option);
    }
    
    public void ClearDialogueOptions()
    {
        _dialogueOptions.Clear();
    }
}