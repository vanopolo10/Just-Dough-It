using System;
using UnityEngine;

public class TalkGate : ITutorialGate
{
    private DialogueManager _dialogueManager;
    private string _textKey;

    public event Action Completed;
    
    public GameObject IconObject { get; }

    public TalkGate(DialogueManager dialogueManager, GameObject iconObject = null, string textKey = "")
    {
        _dialogueManager = dialogueManager;
        IconObject = iconObject;
        _textKey = textKey;
    }

    public void Enter()
    {
        _dialogueManager.DialogueOptionPlayed += OnTalked;
    }

    private void OnTalked(DialogueOption dialogueOption)
    { 
        if (dialogueOption.TextKey != _textKey && _textKey != "")
             return;
        
        _dialogueManager.DialogueOptionPlayed -= OnTalked; 
        Completed?.Invoke();
    }
}