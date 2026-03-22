using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerInteraction",
    menuName = "ScriptableObjects/CustomerSystem/CustomerDialogueSplit")]
public class CustomerDialogueSplit : ScriptableObject
{
    [SerializeField] private bool _useStartSequence = true;
    [SerializeField] private CustomerInteractionSequence _startSequence;
    [SerializeField] private List<DialogueOption> _dialogueOptions = new();
    private bool _useEndSequence = false;
    private CustomerInteractionSequence _endSequence;

    private Action _callback;
    private Customer _customer;

    public void SetCallback(Action callback) { 
        _callback = callback;
    }
    public void SetCustomer(Customer customer) { 
        _customer = customer;
    }

    public void PlayOut() {
        if(_useStartSequence) 
            _startSequence.PlayOut(_customer, InitializeOptions);
        else 
            InitializeOptions();
    }
    public void InitializeOptions() {
        Action callback;
        if (_useEndSequence)
        {
            Debug.Log("[CustomerDialogueSplit] setting up end sequence");
            _endSequence.SetCallback(FinalizeDialogue);
            callback = _endSequence.PlayOutBase;
        }
        else { 
            callback = FinalizeDialogue;
        }

        for (int i = 0; i < _dialogueOptions.Count; i++) {
            _dialogueOptions[i].Interaction.SetCallback(callback);
        }
        Debug.Log("[CustomerDialogueSplit] option callback set to " + callback.Method.Name);

        DisplayOptions();
    }
    public void FinalizeDialogue() {
        _customer.DialogueManager.SetDialogueOptions(null);
        _callback?.Invoke();
    }

    private void DisplayOptions() { 
        _customer.DialogueManager.SetDialogueOptions(_dialogueOptions);
    }
}
