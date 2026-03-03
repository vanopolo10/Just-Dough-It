using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private GameObject _baseDialogueOption, _baseInactiveDialogueOption;
    [SerializeField] private float _dialogueOptionOffset = 1f;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private float _typeSpeed = 0.05f;
    [SerializeField] private bool _skipOnClick = true;
    
    private string _fullText;
    private Coroutine _typeRoutine;
    private bool _isSkipping;
    private bool _isTyping;
    private float _postDisplayDelay;
    private string _pendingText;
    private float _pendingDelay;
    private bool _hasPendingText;
    
    private List<DialogueOption> _dialogueOptions;
    private List<bool> _dialogueOptionActive;
    private List<Transform> _spawnedDialogueOptions = new();
    
    private TMP_TextInfo _textInfo;
    private bool _isMeshInitialized;

    public event Action TextDisplayed;

    private void Awake()
    {
        if (_textMeshPro == null)
            _textMeshPro = _speechBubble.GetComponentInChildren<TextMeshProUGUI>();
            
        SetDialogueOptions(new List<DialogueOption>());

        _textMeshPro.text = string.Empty;
        SetTextAlpha(0f);
    }

    private void Start()
    {
        DisableBubble();
    }
    
    public void DisplayText(string text, float delayAfterTyping = 0f)
    {
        if (_isTyping)
        {
            _pendingText = text;
            _pendingDelay = delayAfterTyping;
            _hasPendingText = true;
            return;
        }
        
        ShowText(text, delayAfterTyping);
        
        if (_speechBubble.activeSelf == false) 
            EnableBubble();
    }

    public void SetDialogueOptions(List<DialogueOption> options)
    {
        _dialogueOptions = options;

        bool[] tmp = new bool[options.Count];
        Array.Fill(tmp, true);
        _dialogueOptionActive = new List<bool>(tmp);

        RefreshDialogueHandles();
    }

    public void DeactivateDialogueOption(DialogueOption option)
    {
        int index = _dialogueOptions.IndexOf(option);
        if (index >= 0 && index < _dialogueOptionActive.Count)
            _dialogueOptionActive[index] = false;
        RefreshDialogueHandles();
    }

    private void ShowText(string text, float delayAfterTyping = 0f)
    {
        _fullText = text;
        _postDisplayDelay = delayAfterTyping;
        _isMeshInitialized = false;

        SetTextAlpha(0f);

        _textMeshPro.text = text;
        
        if (_typeRoutine != null)
            StopCoroutine(_typeRoutine);
            
        _typeRoutine = StartCoroutine(TypeTextRoutine());
    }
    
    private void EnableBubble()
    {
        _speechBubble.SetActive(true);
        RefreshDialogueHandles();
    }

    public void DisableBubble()
    {
        ClearDialogueHandles();
        _speechBubble.SetActive(false);
        _hasPendingText = false;
        _textMeshPro.text = string.Empty;
    }
    
    private void ClearDialogueHandles()
    {
        if (_spawnedDialogueOptions == null) return;
        foreach (Transform child in _spawnedDialogueOptions)
        {
            if (child.gameObject != _baseDialogueOption && child.gameObject != _baseInactiveDialogueOption)
            {
                Destroy(child.gameObject);
            }
        }

        _spawnedDialogueOptions = new List<Transform>();
    }

    private void RefreshDialogueHandles()
    {
        ClearDialogueHandles();

        Vector3 curOffset = Vector3.zero;
        foreach (DialogueOption option in _dialogueOptions)
        {
            int index = _dialogueOptions.IndexOf(option);
            if (index < 0 || index >= _dialogueOptionActive.Count) continue;
            
            var optionObj = _dialogueOptionActive[index]
                ? Instantiate(_baseDialogueOption, _baseDialogueOption.transform.parent)
                : Instantiate(_baseInactiveDialogueOption, _baseInactiveDialogueOption.transform.parent);
            optionObj.SetActive(true);

            _spawnedDialogueOptions.Add(optionObj.transform);

            optionObj.transform.localPosition += curOffset;
            curOffset.y -= _dialogueOptionOffset;

            var handle = optionObj.GetComponent<DialogueOptionHandle>();
            if (handle != null)
            {
                var customerManager = GetComponent<CustomerManager>();
                if (customerManager != null)
                    handle.Setup(customerManager.CurrentCustomer, option);
            }
        }
    }

    private void SetTextAlpha(float alpha)
    {
        if (_textMeshPro == null) return;
        
        Color color = _textMeshPro.color;
        color.a = Mathf.Clamp01(alpha);
        _textMeshPro.color = color;
    }

    private bool InitializeMeshIfNeeded()
    {
        if (_textMeshPro == null) return false;

        _textMeshPro.ForceMeshUpdate();
        _textInfo = _textMeshPro.textInfo;

        if (_textInfo == null || _textInfo.meshInfo == null || _textInfo.meshInfo.Length == 0)
        {
            return false;
        }
        
        _isMeshInitialized = true;
        return true;
    }

    private IEnumerator TypeTextRoutine()
    {
        _isTyping = true;
        _isSkipping = false;

        SetTextAlpha(0f);

        yield return null;

        if (!InitializeMeshIfNeeded())
        {
            Debug.LogError("Failed to initialize text mesh");
            _isTyping = false;
            _typeRoutine = null;
            yield break;
        }

        SetAllCharactersAlpha(0);
        ApplyVertexChanges();
        
        int totalCharacters = _textInfo.characterCount;
        int currentCharIndex = 0;

        while (currentCharIndex < totalCharacters && !_isSkipping)
        {
            if (_skipOnClick && Input.GetMouseButtonDown(0))
            {
                _isSkipping = true;
                break;
            }

            if (currentCharIndex < totalCharacters)
            {
                SetCharacterAlpha(currentCharIndex, 255);
                ApplyVertexChanges();
            }
            
            currentCharIndex++;
            
            yield return new WaitForSeconds(_typeSpeed);
        }

        SetAllCharactersAlpha(255);
        ApplyVertexChanges();

        SetTextAlpha(1f);

        if (_postDisplayDelay > 0f && !_isSkipping)
        {
            yield return new WaitForSeconds(_postDisplayDelay);
        }

        TextDisplayed?.Invoke();
    
        _typeRoutine = null;
        _isTyping = false;
        _isSkipping = false;
        
        CheckForPendingText();
    }

    private void SetAllCharactersAlpha(byte alpha)
    {
        if (!_isMeshInitialized || _textInfo == null) return;
        
        int characterCount = _textInfo.characterCount;
        
        for (int i = 0; i < characterCount; i++)
        {
            SetCharacterAlpha(i, alpha);
        }
    }

    private void SetCharacterAlpha(int charIndex, byte alpha)
    {
        if (!_isMeshInitialized || _textInfo == null) return;
        if (charIndex < 0 || charIndex >= _textInfo.characterCount) return;
        
        var charInfo = _textInfo.characterInfo[charIndex];
        if (!charInfo.isVisible) return;
        
        int materialIndex = charInfo.materialReferenceIndex;
        if (materialIndex < 0 || materialIndex >= _textInfo.meshInfo.Length) return;
        
        int vertexIndex = charInfo.vertexIndex;
        var meshInfo = _textInfo.meshInfo[materialIndex];
        
        if (meshInfo.colors32 == null) return;
        if (vertexIndex < 0 || vertexIndex + 3 >= meshInfo.colors32.Length) return;
        
        Color32 newColor = new Color32(255, 255, 255, alpha);
        
        for (int j = 0; j < 4; j++)
        {
            meshInfo.colors32[vertexIndex + j] = newColor;
        }
    }

    private void ApplyVertexChanges()
    {
        if (!_isMeshInitialized || _textMeshPro == null) return;
        
        try
        {
            _textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to update vertex data: {e.Message}");
        }
    }

    private void CheckForPendingText()
    {
        if (_hasPendingText)
        {
            _hasPendingText = false;
            DisplayText(_pendingText, _pendingDelay);
        }
    }
    
    public void Timeout(float time)
    {
        Invoke(nameof(DisableBubble), time);
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
        _isMeshInitialized = false;
        _textMeshPro.text = string.Empty;
        SetTextAlpha(1f);
    }
}