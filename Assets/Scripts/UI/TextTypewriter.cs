using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private float _typeSpeed = 0.05f;
    
    private string _fullText;
    private Coroutine _typeRoutine;
    private bool _isSkipping;

    public event Action TypingCompleted;

    public bool IsTyping { get; private set; }

    private void Awake()
    {
        if (_textMeshPro == null)
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        
        _textMeshPro.text = string.Empty;
        _textMeshPro.maxVisibleCharacters = 0;
    }

    private void OnEnable()
    {
        Debug.Log("[TextTypewriter] OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log("[TextTypewriter] OnDisable");
        Clear();
    }

    public void StartTyping(string key)
    {
        Debug.Log($"[TextTypewriter] StartTyping called. Text: '{key}', Length: {key.Length}, IsTyping: {IsTyping}");
        
        if (IsTyping)
        {
            Debug.Log("[TextTypewriter] Already typing - stopping previous routine");
            StopCoroutine(_typeRoutine);
            IsTyping = false;
        }

        _fullText = key;
        _textMeshPro.text = key;
        _textMeshPro.maxVisibleCharacters = 0;

        _typeRoutine = StartCoroutine(TypeTextRoutine());
    }

    public void CompleteTypingInstantly()
    {
        Debug.Log($"[TextTypewriter] CompleteTypingInstantly called. IsTyping: {IsTyping}");
        
        if (!IsTyping)
        {
            Debug.Log("[TextTypewriter] Not typing, ignoring");
            return;
        }

        _isSkipping = true;
        _textMeshPro.maxVisibleCharacters = _textMeshPro.text.Length;
        
        if (_typeRoutine != null)
        {
            StopCoroutine(_typeRoutine);
            _typeRoutine = null;
        }
        
        IsTyping = false;
        
        Debug.Log("[TextTypewriter] Text completed instantly, invoking TypingCompleted");
        TypingCompleted?.Invoke();
    }

    private IEnumerator TypeTextRoutine()
    {
        IsTyping = true;
        _isSkipping = false;

        _textMeshPro.ForceMeshUpdate();
        int totalCharacters = _textMeshPro.textInfo.characterCount;
        int currentCharIndex = 0;
        
        Debug.Log($"[TextTypewriter] Starting type routine. Total characters: {totalCharacters}");
        
        while (currentCharIndex < totalCharacters && !_isSkipping)
        {
            currentCharIndex++;
            _textMeshPro.maxVisibleCharacters = currentCharIndex;
            yield return new WaitForSeconds(_typeSpeed);
        }
        
        if (!_isSkipping)
        {
            _textMeshPro.maxVisibleCharacters = totalCharacters;
            Debug.Log("[TextTypewriter] Typing completed naturally, invoking TypingCompleted");
            TypingCompleted?.Invoke();
        }

        _typeRoutine = null;
        IsTyping = false;
        _isSkipping = false;
    }

    public void Clear()
    {
        Debug.Log("[TextTypewriter] Clear");
        
        if (_typeRoutine != null)
        {
            StopCoroutine(_typeRoutine);
            _typeRoutine = null;
        }

        _textMeshPro.text = string.Empty;
        _textMeshPro.maxVisibleCharacters = 0;
        IsTyping = false;
    }
}