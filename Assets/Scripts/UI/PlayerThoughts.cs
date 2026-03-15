using System;
using UnityEngine;

public class PlayerThoughts : MonoBehaviour
{
    [SerializeField] private DialogueBubble _dialogueBubble;
    [SerializeField] private TextTypewriter _textTypewriter;

    public event Action Thought;
    
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

    private void Start()
    {
        _dialogueBubble.gameObject.SetActive(true);
    }

    public void Think(string key)
    {
        _dialogueBubble.gameObject.SetActive(true);
        _textTypewriter.StartTyping(key); //Localize
        
    }

    private void OnTypingCompleted()
    {
        Thought?.Invoke();
    }
    
    private void HandleClick()
    {
        if (_textTypewriter.IsTyping)
            _textTypewriter.CompleteTypingInstantly();
    }
}
