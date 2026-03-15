using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomerInteraction
{
    [SerializeField] private string _dialogueKey;

    private Customer _targetCustomer;
    private CustomerInteraction _nextInteraction;
    
    public string DialogueKey => _dialogueKey;

    public void PlayOut(Customer target)
    {
        Debug.Log($"[CustomerInteraction] PlayOut (single) called. DialogueKey: {_dialogueKey}, Target: {(target != null ? target.name : "null")}");
        PlayOut(target, null);
    }
    
    public void PlayOut(Customer target, CustomerInteraction nextInteraction)
    {
        Debug.Log($"[CustomerInteraction] PlayOut (with next) called. DialogueKey: {_dialogueKey}, Has nextInteraction: {nextInteraction != null}, Target: {(target != null ? target.name : "null")}");
        
        if (!target)
        {
            Debug.LogError("[CustomerInteraction] Target customer is null");
            return;
        }

        DialogueManager dialogueManager = target.DialogueManager;
        
        if (!dialogueManager)
        {
            Debug.LogError("[CustomerInteraction] DialogueManager is null on target customer");
            return;
        }

        _targetCustomer = target;
        _nextInteraction = nextInteraction;

        if (nextInteraction != null)
        {
            Debug.Log($"[CustomerInteraction] Next interaction exists. Creating callback to play next interaction: {nextInteraction.DialogueKey}");
            dialogueManager.DisplayTextWithCallback(_dialogueKey, OnTextCompleted);
        }
        else
        {
            Debug.Log($"[CustomerInteraction] No next interaction. Displaying text without callback");
            dialogueManager.DisplayText(_dialogueKey);
        }
    }

    private void OnTextCompleted()
    {
        Debug.Log($"[CustomerInteraction] OnTextCompleted called. Current dialogue: {_dialogueKey}, Has nextInteraction: {_nextInteraction != null}");
        
        if (!_targetCustomer || !_targetCustomer.DialogueManager)
        {
            Debug.LogError("[CustomerInteraction] Target customer or DialogueManager is null in OnTextCompleted");
            Cleanup();
            return;
        }

        if (_nextInteraction != null)
        {
            Debug.Log($"[CustomerInteraction] Playing next interaction: {_nextInteraction.DialogueKey}");
            _nextInteraction.PlayOut(_targetCustomer);
        }
        else
        {
            Debug.Log("[CustomerInteraction] No next interaction - chain ended");
        }

        Cleanup();
    }

    private void Cleanup()
    {
        Debug.Log($"[CustomerInteraction] Cleanup for dialogue: {_dialogueKey}");
        _targetCustomer = null;
        _nextInteraction = null;
    }
}

[Serializable]
public struct DialogueOption : IEquatable<DialogueOption>
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

    public bool Equals(DialogueOption other)
    {
        return _textKey == other._textKey && Equals(_interaction, other._interaction);
    }

    public override bool Equals(object obj)
    {
        return obj is DialogueOption other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_textKey, _interaction);
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