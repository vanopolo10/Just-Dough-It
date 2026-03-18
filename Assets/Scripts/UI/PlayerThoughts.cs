using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerThoughts : MonoBehaviour
{
    [SerializeField] private DialogueBubble _dialogueBubble;
    [SerializeField] private TextTypewriter _textTypewriter;
    [SerializeField] private Image _icon;
    
    private bool _waitingForClose;
    
    public event Action ThoughtCompleted;
    
    private void Awake()
    {
        _dialogueBubble.gameObject.SetActive(false);
        _icon.gameObject.SetActive(false);
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

    public void Think(string key, bool doShowIcon = false)
    {
        _waitingForClose = false;

        _dialogueBubble.gameObject.SetActive(true);
        _ = _textTypewriter.StartTyping(key);

        if (doShowIcon)
            _icon.gameObject.SetActive(true);
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

            _icon.gameObject.SetActive(false);
            ThoughtCompleted?.Invoke();
        }
    }
}