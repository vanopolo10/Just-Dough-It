using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private bool _isTutorial;

    [SerializeField] private DialogueBubble _speechBubble;
    [SerializeField] private TextTypewriter _typewriter;

    [SerializeField] private GameObject _baseDialogueOption;
    [SerializeField] private GameObject _baseInactiveDialogueOption;

    [SerializeField] private float _dialogueOptionOffset = 1f;

    private List<DialogueOptionData> _dialogueOptions = new();
    private readonly List<Transform> _spawnedDialogueOptions = new();

    private Action _onTypingCompleted;
    private Action _onTextClicked;

    private bool _isFinalQuestText;

    public event Action SkipClicked;
    public event Action ConfirmClicked;
    public event Action TypingCompleted;
    public event Action<DialogueOption> DialogueOptionPlayed;

    public bool IsTextFullyVisible { get; private set; }
    
    [Serializable]
    private class DialogueOptionData
    {
        public DialogueOption Option;
        public bool IsActive;

        public DialogueOptionData(DialogueOption option, bool isActive = true)
        {
            Option = option;
            IsActive = isActive;
        }
    }
    
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
        Debug.Log("[DialogueManager] OnEnable");
        if (_speechBubble != null)
            _speechBubble.OnBubbleClicked += HandleClick;

        SubscribeToTypewriter();
    }

    private void OnDisable()
    {
        Debug.Log("[DialogueManager] OnDisable");
        if (_speechBubble != null)
            _speechBubble.OnBubbleClicked -= HandleClick;

        UnsubscribeFromTypewriter();
    }

    private void SubscribeToTypewriter()
    {
        if (_typewriter != null)
        {
            _typewriter.TypingCompleted += OnTypingCompleted;
        }
    }

    private void UnsubscribeFromTypewriter()
    {
        if (_typewriter != null)
        {
            _typewriter.TypingCompleted -= OnTypingCompleted;
        }
    }

    private void HandleClick()
    {
        if (_typewriter == null)
            return;
        
        if (_typewriter.IsTyping)
        {
            _typewriter.CompleteTypingInstantly();
            SkipClicked?.Invoke();
            return;
        }

        if (IsTextFullyVisible)
        {
            ConfirmClicked?.Invoke();

            var clickCallback = _onTextClicked;
            bool wasFinal = _isFinalQuestText;

            IsTextFullyVisible = false;
            _onTypingCompleted = null;
            _onTextClicked = null;
            _isFinalQuestText = false;

            if (clickCallback != null)
                clickCallback.Invoke();

            if (wasFinal)
                SetDialogueOptions(new List<DialogueOption>());
        }
    }

    private void OnTypingCompleted()
    {
        IsTextFullyVisible = true;
        
        _onTypingCompleted?.Invoke();
        TypingCompleted?.Invoke();
    }

    private void EnableBubble()
    {
        if (!_speechBubble.gameObject.activeSelf)
            _speechBubble.gameObject.SetActive(true);
    }

    public void DisableBubble()
    {
        IsTextFullyVisible = false;
        _onTypingCompleted = null;
        _onTextClicked = null;
        _isFinalQuestText = false;

        ClearDialogueHandles();
        
        if (_speechBubble != null)
            _speechBubble.gameObject.SetActive(false);
            
        if (_typewriter != null)
            _typewriter.Clear();

        SetDialogueOptions(new List<DialogueOption>());
    }

    public void DisplayText(string text)
    { 
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = null;
        _onTextClicked = null;
        _isFinalQuestText = false;

        _ = _typewriter.StartTyping(text);
    }

    public void DisplayTextWithTypingCallback(string text, Action onTypingCompleted)
    {
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = onTypingCompleted;
        _onTextClicked = null;
        _isFinalQuestText = false;

        _ = _typewriter.StartTyping(text);
    }

    public void DisplayTextWithClickCallback(string text, Action onTextClicked)
    {
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = null;
        _onTextClicked = onTextClicked;
        _isFinalQuestText = false;

        _ = _typewriter.StartTyping(text);
    }

    public void DisplayTextWithCallbacks(string text, Action onTypingCompleted, Action onTextClicked)
    {
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = onTypingCompleted;
        _onTextClicked = onTextClicked;
        _isFinalQuestText = false;

        _ = _typewriter.StartTyping(text);
    }

    public void DisplayFinalQuestText(string text, Action onTextClicked)
    {
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = null;
        _onTextClicked = onTextClicked;
        _isFinalQuestText = true;

        _ = _typewriter.StartTyping(text);
    }

    public void DisplayTextWithCallback(string text, Action onComplete)
    {
        DisplayTextWithClickCallback(text, onComplete);
    }

    public void SetDialogueOptions(List<DialogueOption> options)
    {
        _dialogueOptions.Clear();
        
        if (options != null)
            foreach (var option in options)
                _dialogueOptions.Add(new DialogueOptionData(option));
        
        RefreshDialogueHandles();
    }

    public void DeactivateDialogueOption(DialogueOption option)
    {
        var optionData = _dialogueOptions.FirstOrDefault(od => od.Option.Equals(option));
        if (optionData != null)
        {
            optionData.IsActive = false;
            RefreshDialogueHandles();
        }
        
        DialogueOptionPlayed?.Invoke(option);
    }
    
    public void ActivateDialogueOption(DialogueOption option)
    {
        var optionData = _dialogueOptions.FirstOrDefault(od => od.Option.Equals(option));
        if (optionData != null)
        {
            optionData.IsActive = true;
            RefreshDialogueHandles();
        }
    }
    
    private void ClearDialogueHandles()
    {
        foreach (var t in _spawnedDialogueOptions.Where(t => t != null))
            Destroy(t.gameObject);

        _spawnedDialogueOptions.Clear();
    }

    private void RefreshDialogueHandles()
    {
        if (_speechBubble == null || !_speechBubble.gameObject.activeSelf)
            return;

        ClearDialogueHandles();

        Vector3 offset = Vector3.zero;

        foreach (var optionData in _dialogueOptions)
        {
            GameObject prefab = optionData.IsActive
                ? _baseDialogueOption
                : _baseInactiveDialogueOption;

            var optionObj = Instantiate(prefab, prefab.transform.parent);
            optionObj.SetActive(true);
            optionObj.transform.localPosition += offset;
            offset.y -= _dialogueOptionOffset;

            _spawnedDialogueOptions.Add(optionObj.transform);

            var handle = optionObj.GetComponent<DialogueOptionHandle>();
            if (handle != null)
            {
                var customerManager = GetComponent<CustomerManager>();
                if (customerManager != null)
                    _ = handle.Setup(customerManager.CurrentCustomer, optionData.Option);
            }
        }
    }
}