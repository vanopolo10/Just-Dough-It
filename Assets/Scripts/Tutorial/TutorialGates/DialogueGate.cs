using System;
using UnityEngine;

public class DialogueGate : ITutorialGate
{
    private readonly DialogueManager _dialogue;
    private readonly string _textKey;
    private bool _isOnConfirm;

    public event Action Completed;

    public GameObject IconObject { get; }

    public DialogueGate(DialogueManager dialogue, bool isOnConfirm, GameObject iconObject = null)
    {
        _isOnConfirm = isOnConfirm;
        _dialogue = dialogue;
        IconObject = iconObject;
    }

    public void Enter()
    {
        if (_isOnConfirm)
            _dialogue.ConfirmClicked += OnTalked;
        else
            _dialogue.TypingCompleted += OnTalked;
    }

    private void OnTalked()
    {
        _dialogue.ConfirmClicked -= OnTalked;
        _dialogue.ConfirmClicked -= OnTalked;
        Completed?.Invoke();
    }
}