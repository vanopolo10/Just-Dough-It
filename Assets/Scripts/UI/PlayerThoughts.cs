using System;
using UnityEngine;

public class PlayerThoughts : MonoBehaviour
{
    [SerializeField] private DialogueBubble _dialogueBubble;
    [SerializeField] private TextTypewriter _textTypewriter;
    
    private bool _waitingForClose;
    
    public event Action ThoughtCompleted;
    
    private void Awake()
    {
        _dialogueBubble.gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        _dialogueBubble.OnBubbleClicked += HandleClick;
        _textTypewriter.TypingCompleted += OnTypingCompleted;
    }

    private void OnDisable()
    {
        _dialogueBubble.OnBubbleClicked -= HandleClick;
        _textTypewriter.TypingCompleted -= OnTypingCompleted;
    }

    public void Think(string key)
    {
        _waitingForClose = false;

        _dialogueBubble.gameObject.SetActive(true);
        _textTypewriter.StartTyping(key);
    }

    private void OnTypingCompleted()
    {
        _waitingForClose = true;
    }

    private void HandleClick()
    {
        if (_textTypewriter.IsTyping)
        {
            _textTypewriter.CompleteTypingInstantly();
            return;
        }

        if (_waitingForClose)
        {
            _dialogueBubble.gameObject.SetActive(false);
            _waitingForClose = false;

            ThoughtCompleted?.Invoke();
        }
    }
}