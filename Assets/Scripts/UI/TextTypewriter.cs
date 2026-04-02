using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class TextTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private float _typeSpeed = 0.05f;

    private string _textKey;
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
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChange;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChange;
        Clear();
    }

    public async Task StartTyping(string key, string add = "")
    {
        Debug.Log($"[TextTypewriter] StartTyping called. Text: '{key}', Length: {key.Length}, IsTyping: {IsTyping}");

        if (IsTyping)
        {
            Debug.Log("[TextTypewriter] Already typing - stopping previous routine");
            StopCoroutine(_typeRoutine);
            IsTyping = false;
        }

        _textKey = key;
        var task = LocalizationSettingsExtension.FindStringInAllTablesAsync(_textKey);
        await task;
        _textMeshPro.text = task.Result + add;
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
        if (_typeRoutine != null)
        {
            StopCoroutine(_typeRoutine);
            _typeRoutine = null;
        }

        _textMeshPro.text = string.Empty;
        _textMeshPro.maxVisibleCharacters = 0;
        IsTyping = false;
    }

    private void OnLocaleChange(Locale locale)
    {
        _ = SelectedLocaleChanged();
    }

    private async Task SelectedLocaleChanged()
    {
        float displayedPercent = _textMeshPro.maxVisibleCharacters / _textMeshPro.text.Length;
        var task = LocalizationSettingsExtension.FindStringInAllTablesAsync(_textKey);
        await task;
        _textMeshPro.text = task.Result;
        _textMeshPro.maxVisibleCharacters = Mathf.FloorToInt(_textMeshPro.text.Length * displayedPercent); 
    }
}