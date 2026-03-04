using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomerInteraction
{
    [SerializeField] private string _dialogueKey;
    [SerializeField] private float _postDisplayDelay = 0f;

    private bool _isWaitingForTextDisplay;
    private Customer _targetCustomer;
    private CustomerInteraction _nextInteraction;
    private float _nextDelay;
    
    public string DialogueKey => _dialogueKey;
    public float PostDisplayDelay => _postDisplayDelay;

    public void PlayOut(Customer target)
    {
        PlayOut(target, null, 0f);
    }
    
    public void PlayOut(Customer target, CustomerInteraction nextInteraction, float nextDelay = 0f)
    {
        if (!target)
        {
            Debug.LogError("Target customer is null");
            return;
        }

        DialogueManager dialogueManager = target.DialogueManager;
        
        if (!dialogueManager)
        {
            Debug.LogError("DialogueManager is null on target customer");
            return;
        }

        if (_isWaitingForTextDisplay)
        {
            Debug.LogWarning($"Interaction {_dialogueKey} is already waiting for TextDisplayed");
            return;
        }

        _targetCustomer = target;
        _nextInteraction = nextInteraction;
        _nextDelay = nextDelay;

        if (nextInteraction != null)
        {
            dialogueManager.TextDisplayed += OnTextDisplayed;
            _isWaitingForTextDisplay = true;
        }

        dialogueManager.DisplayText(_dialogueKey, _postDisplayDelay);
    }

    private void OnTextDisplayed()
    {
        if (!_targetCustomer || !_targetCustomer.DialogueManager)
        {
            Cleanup();
            return;
        }

        _targetCustomer.DialogueManager.TextDisplayed -= OnTextDisplayed;

        _nextInteraction?.PlayOut(_targetCustomer);

        Cleanup();
    }

    private void Cleanup()
    {
        _isWaitingForTextDisplay = false;
        _targetCustomer = null;
        _nextInteraction = null;
        _nextDelay = 0f;
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