using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueBubble _speechBubble;
    [SerializeField] private TextTypewriter _typewriter;
    [SerializeField] private GameObject _baseDialogueOption, _baseInactiveDialogueOption;
    [SerializeField] private float _dialogueOptionOffset = 1f;
    
    private List<DialogueOption> _dialogueOptions;
    private List<bool> _dialogueOptionActive;
    private List<Transform> _spawnedDialogueOptions = new();

    private bool _isTextFullyVisible = false;
    private Action _onCompleteCurrentText;
    private bool _isFinalQuestText = false;

    public TextTypewriter Typewriter => _typewriter;

    private void Awake()
    {
        if (_typewriter == null)
            _typewriter = GetComponent<TextTypewriter>();

        SetDialogueOptions(new List<DialogueOption>());
    }

    private void Start()
    {
        DisableBubble();
    }

    private void OnEnable()
    {
        if (_speechBubble != null)
            _speechBubble.OnBubbleClicked += HandleClick;
    }

    private void OnDisable()
    {
        if (_speechBubble != null)
            _speechBubble.OnBubbleClicked -= HandleClick;
            
        if (_typewriter != null)
        {
            _typewriter.TypingCompleted -= OnTypingCompleted;
            _typewriter.Clear();
        }
    }

    private void HandleClick()
    {
        Debug.Log($"[DialogueManager] HandleClick called. IsTyping: {_typewriter.IsTyping}, IsTextFullyVisible: {_isTextFullyVisible}, IsFinalQuestText: {_isFinalQuestText}");
        
        if (_typewriter.IsTyping)
        {
            Debug.Log("[DialogueManager] Case 1: Text is typing - completing instantly");
            _typewriter.CompleteTypingInstantly();
            _isTextFullyVisible = true;
            Debug.Log($"[DialogueManager] Text completed instantly. IsTextFullyVisible set to: {_isTextFullyVisible}");
        }
        else if (_isTextFullyVisible)
        {
            Debug.Log("[DialogueManager] Case 2: Text is fully visible - executing callback");

            var callback = _onCompleteCurrentText;
            bool wasFinalText = _isFinalQuestText;

            _isTextFullyVisible = false;
            _onCompleteCurrentText = null;
            _isFinalQuestText = false;
            
            if (callback != null)
            {
                Debug.Log("[DialogueManager] Callback exists - invoking");

                if (wasFinalText)
                {
                    Debug.Log("[DialogueManager] This is final quest text - clearing dialogue options");
                    SetDialogueOptions(new List<DialogueOption>());
                }
                
                callback.Invoke();
            }
            else
            {
                Debug.LogWarning("[DialogueManager] Callback is null! No action to execute");
            }
        }
        else
        {
            Debug.LogWarning($"[DialogueManager] Case 3: Unexpected state - IsTyping: {_typewriter.IsTyping}, IsTextFullyVisible: {_isTextFullyVisible}");
        }
    }

    private void OnTypingCompleted()
    {
        _isTextFullyVisible = true;
        _typewriter.TypingCompleted -= OnTypingCompleted;
    }

    public void DisplayText(string text)
    {
        if (!_speechBubble.gameObject.activeSelf)
            EnableBubble();
        
        _isTextFullyVisible = false;
        _onCompleteCurrentText = null;
        _isFinalQuestText = false;

        _typewriter.TypingCompleted += OnTypingCompleted;
        
        _typewriter.StartTyping(text);
    }

    public void DisplayTextWithCallback(string text, Action onComplete)
    {
        DisplayText(text);
        _onCompleteCurrentText = onComplete;
    }

    public void DisplayFinalQuestText(string text, Action onComplete)
    {
        if (!_speechBubble.gameObject.activeSelf)
            EnableBubble();
        
        _isTextFullyVisible = false;
        _onCompleteCurrentText = onComplete;
        _isFinalQuestText = true;

        _typewriter.TypingCompleted += OnTypingCompleted;
        
        _typewriter.StartTyping(text);
        
        Debug.Log("[DialogueManager] Displaying final quest text");
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

    private void EnableBubble()
    {
        _speechBubble.gameObject.SetActive(true);
        RefreshDialogueHandles();
    }

    public void DisableBubble()
    {
        Debug.Log("[DialogueManager] Disabling bubble");
        
        if (_typewriter != null)
        {
            _typewriter.TypingCompleted -= OnTypingCompleted;
            _typewriter.Clear();
        }
        
        ClearDialogueHandles();
        _speechBubble.gameObject.SetActive(false);
        _typewriter.Clear();
        _isTextFullyVisible = false;
        _onCompleteCurrentText = null;
        _isFinalQuestText = false;

        SetDialogueOptions(new List<DialogueOption>());
    }

    private void ClearDialogueHandles()
    {
        if (_spawnedDialogueOptions == null) return;
        
        foreach (var child in _spawnedDialogueOptions.Where(child => child.gameObject != _baseDialogueOption && child.gameObject != _baseInactiveDialogueOption))
            Destroy(child.gameObject);
        
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
            if (handle)
            {
                var customerManager = GetComponent<CustomerManager>();
                if (customerManager)
                    handle.Setup(customerManager.CurrentCustomer, option);
            }
        }
    }
}