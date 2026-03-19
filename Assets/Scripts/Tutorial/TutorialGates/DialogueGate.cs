using System;
using UnityEngine;

public class DialogueGate : ITutorialGate
{
    private readonly DialogueManager _dialogue;
    private readonly string _textKey;

    public event Action Completed;

    public GameObject IconObject { get; }

    public DialogueGate(DialogueManager dialogue, GameObject iconObject = null)
    {
        _dialogue = dialogue;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _dialogue.ConfirmClicked += OnConfirmClicked;
    }

    private void OnConfirmClicked()
    {
        _dialogue.ConfirmClicked -= OnConfirmClicked;
        Completed?.Invoke();
    }
}