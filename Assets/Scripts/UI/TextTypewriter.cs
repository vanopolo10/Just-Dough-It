using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private float _typeSpeed = 0.05f;
    //[SerializeField] private bool _isRevealMode; мб сделаю
    
    private string _fullText;
    private Coroutine _typeRoutine;
    private bool _isTyping;
    private bool _isSkipping;

    public event Action TypingCompleted;

    public bool IsTyping => _isTyping;

    private void Awake()
    {
        if (_textMeshPro == null)
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        
        _textMeshPro.text = string.Empty;
        _textMeshPro.maxVisibleCharacters = 0;
    }

    public void StartTyping(string key) //@deer_rus localize
    {
        Debug.Log($"[TextTypewriter] StartTyping called. Text length: {key.Length}. Is currently typing: {_isTyping}");
        
        if (_isTyping)
        {
            Debug.Log("[TextTypewriter] Already typing - stopping previous routine");
            StopCoroutine(_typeRoutine);
            _isTyping = false;
        }

        _fullText = key;
        _textMeshPro.text = key;
        _textMeshPro.maxVisibleCharacters = 0;

        _typeRoutine = StartCoroutine(TypeTextRoutine());
    }

    public void CompleteTypingInstantly()
    {
        Debug.Log($"[TextTypewriter] CompleteTypingInstantly called. IsTyping: {_isTyping}");
        
        if (!_isTyping) return;

        _isSkipping = true;
        _textMeshPro.maxVisibleCharacters = _fullText.Length;
        Debug.Log("[TextTypewriter] Text completed instantly");
    }

    private IEnumerator TypeTextRoutine()
    {
        _isTyping = true;
        _isSkipping = false;

        _textMeshPro.ForceMeshUpdate();
        int totalCharacters = _textMeshPro.textInfo.characterCount;
        int currentCharIndex = 0;
        
        while (currentCharIndex < totalCharacters && !_isSkipping)
        {
            currentCharIndex++;
            _textMeshPro.maxVisibleCharacters = currentCharIndex;
            yield return new WaitForSeconds(_typeSpeed);
        }
        
        _textMeshPro.maxVisibleCharacters = totalCharacters;

        if (!_isSkipping)
            TypingCompleted?.Invoke();

        _typeRoutine = null;
        _isTyping = false;
        _isSkipping = false;
    }

    public void Clear()
    {
        Debug.Log("[TextTypewriter] Clear called");
        
        if (_typeRoutine != null)
        {
            StopCoroutine(_typeRoutine);
            _typeRoutine = null;
        }

        _textMeshPro.text = string.Empty;
        _textMeshPro.maxVisibleCharacters = 0;
        _isTyping = false;
    }

    private void OnDisable()
    {
        Clear();
    }
}