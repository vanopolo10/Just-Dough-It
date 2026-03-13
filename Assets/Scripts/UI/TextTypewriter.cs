using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private float _typeSpeed = 0.05f;
    [SerializeField] private bool _skipOnClick = true;

    private string _fullText;
    private Coroutine _typeRoutine;
    private bool _isTyping;
    private bool _isSkipping;
    private float _postDisplayDelay;
    private string _pendingText;
    private float _pendingDelay;
    private bool _hasPendingText;

    public event Action TextDisplayed;

    private void Awake()
    {
        if (_textMeshPro == null)
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        
        _textMeshPro.text = string.Empty;
        _textMeshPro.maxVisibleCharacters = 0;
    }

    public void StartTyping(string text, float delayAfterTyping = 0f)
    {
        if (_isTyping)
        {
            _pendingText = text;
            _pendingDelay = delayAfterTyping;
            _hasPendingText = true;
            return;
        }

        ShowText(text, delayAfterTyping);
    }

    public void ShowTextImmediately(string text)
    {
        if (_typeRoutine != null)
            StopCoroutine(_typeRoutine);

        _textMeshPro.text = text;
        _textMeshPro.maxVisibleCharacters = text.Length;
        _textMeshPro.ForceMeshUpdate();
        _textMeshPro.maxVisibleCharacters = _textMeshPro.textInfo.characterCount;

        _isTyping = false;
        _hasPendingText = false;
        _typeRoutine = null;
    }

    public void Clear()
    {
        if (_typeRoutine != null)
        {
            StopCoroutine(_typeRoutine);
            _typeRoutine = null;
        }

        _textMeshPro.text = string.Empty;
        _textMeshPro.maxVisibleCharacters = 0;
        _isTyping = false;
        _hasPendingText = false;
    }

    private void ShowText(string text, float delayAfterTyping)
    {
        _fullText = text;
        _postDisplayDelay = delayAfterTyping;

        _textMeshPro.text = text;
        _textMeshPro.maxVisibleCharacters = 0;

        if (_typeRoutine != null)
            StopCoroutine(_typeRoutine);
        
        _typeRoutine = StartCoroutine(TypeTextRoutine());
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
            if (_skipOnClick && Input.GetMouseButtonDown(0))
            {
                _isSkipping = true;
                break;
            }

            currentCharIndex++;
            _textMeshPro.maxVisibleCharacters = currentCharIndex;

            yield return new WaitForSeconds(_typeSpeed);
        }

        _textMeshPro.maxVisibleCharacters = totalCharacters;

        if (_postDisplayDelay > 0f && !_isSkipping)
            yield return new WaitForSeconds(_postDisplayDelay);

        TextDisplayed?.Invoke();

        _typeRoutine = null;
        _isTyping = false;
        _isSkipping = false;

        CheckForPendingText();
    }

    private void CheckForPendingText()
    {
        if (_hasPendingText)
        {
            _hasPendingText = false;
            ShowText(_pendingText, _pendingDelay);
        }
    }

    private void OnDisable()
    {
        if (_typeRoutine != null)
        {
            StopCoroutine(_typeRoutine);
            _typeRoutine = null;
        }

        _isTyping = false;
        _hasPendingText = false;
        _textMeshPro.text = string.Empty;
        _textMeshPro.maxVisibleCharacters = 0;
    }
}