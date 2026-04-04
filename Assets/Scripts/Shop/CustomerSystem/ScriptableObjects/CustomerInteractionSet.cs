using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomerInteraction
{
    [SerializeField] private string _dialogueKey;

    private Customer _targetCustomer;
    private CustomerInteraction _nextInteraction;
    private Action _callback;

    public string DialogueKey => _dialogueKey;

    public CustomerInteraction(String dialogueKey = "")
    {
        _dialogueKey = dialogueKey;
    }

    public void SetCallback(Action callback)
    {
        _callback = callback;
        Debug.Log($"[CustomerInteraction] Set a new callback for interaction with text {_dialogueKey}");
    }

    public void SetNextInteraction(CustomerInteraction nextInteraction)
    {
        Debug.Log(
            $"[CustomerInteraction] SetNextInteraction set. Current dialogue: {_dialogueKey}, Next dialogue: {(nextInteraction != null ? nextInteraction.DialogueKey : "null")}");
        _nextInteraction = nextInteraction;
    }

    public virtual void PlayOut(Customer target)
    {
        PlayOut(target, _nextInteraction);
    }

    public virtual void PlayOut(Customer target, CustomerInteraction nextInteraction)
    {
        Debug.Log(
            $"[CustomerInteraction] PlayOut (with next) called. DialogueKey: {_dialogueKey}, Has nextInteraction: {nextInteraction != null}, Target: {(target != null ? target.name : "null")}");

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

        dialogueManager.SetCurrentCustomer(target);

        _targetCustomer = target;
        _nextInteraction = nextInteraction;

        if (nextInteraction != null)
        {
            Debug.Log(
                $"[CustomerInteraction] Next interaction exists. Creating callback to play next interaction: {nextInteraction.DialogueKey}");
            dialogueManager.DisplayTextWithClickCallback(_dialogueKey, OnTextClicked);
        }
        else if (_callback != null)
        {
            Debug.Log($"[CustomerInteraction] No next interaction but callback exists. Displaying text with callback");
            dialogueManager.DisplayTextWithCallback(_dialogueKey, _callback);
        }
        else
        {
            Debug.Log($"[CustomerInteraction] No next interaction. Displaying text without callback");
            dialogueManager.DisplayText(_dialogueKey);
        }
    }

    private void OnTextClicked()
    {
        Debug.Log(
            $"[CustomerInteraction] OnTextClicked called. Current dialogue: {_dialogueKey}, Has nextInteraction: {_nextInteraction != null}");

        if (!_targetCustomer || !_targetCustomer.DialogueManager)
        {
            Debug.LogError("[CustomerInteraction] Target customer or DialogueManager is null in OnTextClicked");
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
        //_nextInteraction = null;
    }
}

[Serializable]
public class AnimatedCustomerInteraction : CustomerInteraction
{
    [SerializeField] private string _animationTrigger;

    public AnimatedCustomerInteraction(string dialogueKey = "", string animationTrigger = "") : base(dialogueKey)
    {
        _animationTrigger = animationTrigger;
        Debug.Log(
            $"[AnimatedCustomerInteraction] Created with dialogueKey: {dialogueKey} and animationTrigger: {_animationTrigger}");
    }

    public override void PlayOut(Customer target)
    {
        if (_animationTrigger != "")
            target.AnimatorController.SetCustomTrigger(_animationTrigger);

        base.PlayOut(target);
    }

    public override void PlayOut(Customer target, CustomerInteraction nextInteraction)
    {
        if (_animationTrigger != "")
            target.AnimatorController.SetCustomTrigger(_animationTrigger);
        base.PlayOut(target, nextInteraction);
    }
}

[Serializable]
public class CustomerInteractionSequence
{
    [SerializeField] private List<String> _interactionTexts;
    [SerializeField] private List<String> _animationTriggers;

    //[SerializeField] private CustomerDialogueSplit _dialogueSplit = null;
    private List<AnimatedCustomerInteraction> _interactions;
    private Action _callback = null;
    private Customer _customer = null;

    public bool IsValid() { 
        return _interactionTexts != null && _interactionTexts.Count > 0;
    }
    private void Initialize(Action callback)
    {
        //Debug.Log("Initializing interaction Sequence");
        _interactions = new List<AnimatedCustomerInteraction>();

        while (_animationTriggers.Count < _interactionTexts.Count)
            _animationTriggers.Add("");

        if (_interactionTexts.Count != 0)
        {
            for (int i = 0; i < _interactionTexts.Count; i++)
                _interactions.Add(new AnimatedCustomerInteraction(_interactionTexts[i], _animationTriggers[i]));

            for (int i = 0; i < _interactionTexts.Count - 1; i++)
                _interactions[i].SetNextInteraction(_interactions[i + 1]);

            //if (_dialogueSplit == null)
            if (true)
                _interactions[_interactionTexts.Count - 1].SetCallback(callback);
            /*else
            {
                _interactions[_interactionTexts.Count - 1].SetCallback(_dialogueSplit.PlayOut);
                _dialogueSplit.SetCallback(callback);
                Debug.Log("InteractionSequence Dialogue split detected. Redirecting callback to it instead of last interaction.");
            }
            */
        }
        else
        {
            _interactions.Add(new AnimatedCustomerInteraction());
        }
    }

    public void SetCallback(Action callback)
    {
        _callback = callback;
    }

    public void SetCustomer(Customer customer)
    {
        _customer = customer;
    }

    public void PlayOutBase()
    {
        PlayOut();
    }

    public void PlayOut(Customer target = null, Action callback = null)
    {
        if (callback != null)
            _callback = callback;
        if (target != null)
            _customer = target;

        Initialize(_callback);

        //_dialogueSplit?.SetCustomer(_customer);

        _interactions[0].PlayOut(_customer);
    }
}

[Serializable]
public struct DialogueOption : IEquatable<DialogueOption>
{
    [SerializeField] private string _textKey;
    [SerializeField] private CustomerInteractionSequence _interaction;

    public string TextKey => _textKey;
    public CustomerInteractionSequence Interaction => _interaction;

    public DialogueOption(string textKey, CustomerInteractionSequence interaction)
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
    [SerializeField] private CustomerInteractionSequence _onGreeting;
    [SerializeField] private CustomerInteractionSequence _onItemAccepted;
    [SerializeField] private CustomerInteractionSequence _onItemRejected;
    [SerializeField] private CustomerInteractionSequence _onQuestCompleted;
    [SerializeField] private List<DialogueOption> _questDialogueOptions = new();
    [SerializeField] private Vector2 _voicePitchRange = Vector2.one;
    [SerializeField] private float _voiceSpeed = 4;

    public CustomerInteractionSequence OnGreeting => _onGreeting;
    public CustomerInteractionSequence OnItemAccepted => _onItemAccepted;
    public CustomerInteractionSequence OnItemRejected => _onItemRejected;
    public CustomerInteractionSequence OnQuestCompleted => _onQuestCompleted;

    public List<DialogueOption> DialogueOptions => _questDialogueOptions;

    public float Pitch => UnityEngine.Random.Range(_voicePitchRange.x, _voicePitchRange.y);
    public float Speed => _voiceSpeed;

    public void AddDialogueOption(DialogueOption option)
    {
        _questDialogueOptions.Add(option);
    }

    public void RemoveDialogueOption(DialogueOption option)
    {
        _questDialogueOptions.Remove(option);
    }

    public void ClearDialogueOptions()
    {
        _questDialogueOptions.Clear();
    }
}