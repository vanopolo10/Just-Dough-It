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
            Debug.Log("[DialogueManager] Subscribing to Typewriter events");
            _typewriter.TypingCompleted += OnTypingCompleted;
        }
    }

    private void UnsubscribeFromTypewriter()
    {
        if (_typewriter != null)
        {
            Debug.Log("[DialogueManager] Unsubscribing from Typewriter events");
            _typewriter.TypingCompleted -= OnTypingCompleted;
        }
    }

    private void HandleClick()
    {
        Debug.Log($"[DialogueManager] HandleClick. IsTyping: {(_typewriter != null ? _typewriter.IsTyping.ToString() : "null")}, IsTextFullyVisible: {IsTextFullyVisible}");
        
        if (_typewriter == null)
        {
            Debug.LogError("[DialogueManager] Typewriter is null!");
            return;
        }
        
        if (_typewriter.IsTyping)
        {
            Debug.Log("[DialogueManager] Skipping typing...");
            _typewriter.CompleteTypingInstantly();
            SkipClicked?.Invoke();
            return;
        }

        if (IsTextFullyVisible)
        {
            Debug.Log($"[DialogueManager] Text is fully visible, invoking click callback. Has click callback: {(_onTextClicked != null)}");
            
            ConfirmClicked?.Invoke();

            var clickCallback = _onTextClicked;
            bool wasFinal = _isFinalQuestText;

            IsTextFullyVisible = false;
            _onTypingCompleted = null;
            _onTextClicked = null;
            _isFinalQuestText = false;

            if (clickCallback != null)
            {
                Debug.Log("[DialogueManager] Invoking click callback");
                clickCallback.Invoke();
            }
            else
            {
                Debug.Log("[DialogueManager] No click callback registered");
            }

            if (wasFinal)
            {
                Debug.Log("[DialogueManager] Was final quest text, clearing options");
                SetDialogueOptions(new List<DialogueOption>());
            }
        }
        else
        {
            Debug.Log("[DialogueManager] Text is not fully visible yet, ignoring click");
        }
    }

    private void OnTypingCompleted()
    {
        Debug.Log($"[DialogueManager] OnTypingCompleted received! Has typing callback: {(_onTypingCompleted != null)}");
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
        Debug.Log("[DialogueManager] DisableBubble");

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
        Debug.Log($"[DialogueManager] DisplayText: {text}");
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = null;
        _onTextClicked = null;
        _isFinalQuestText = false;

        _typewriter.StartTyping(text);
    }

    public void DisplayTextWithTypingCallback(string text, Action onTypingCompleted)
    {
        Debug.Log($"[DialogueManager] DisplayTextWithTypingCallback: {text}");
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = onTypingCompleted;
        _onTextClicked = null;
        _isFinalQuestText = false;

        _typewriter.StartTyping(text);
    }

    public void DisplayTextWithClickCallback(string text, Action onTextClicked)
    {
        Debug.Log($"[DialogueManager] DisplayTextWithClickCallback: {text}");
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = null;
        _onTextClicked = onTextClicked;
        _isFinalQuestText = false;

        _typewriter.StartTyping(text);
    }

    public void DisplayTextWithCallbacks(string text, Action onTypingCompleted, Action onTextClicked)
    {
        Debug.Log($"[DialogueManager] DisplayTextWithCallbacks: {text}");
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = onTypingCompleted;
        _onTextClicked = onTextClicked;
        _isFinalQuestText = false;

        _typewriter.StartTyping(text);
    }

    public void DisplayFinalQuestText(string text, Action onTextClicked)
    {
        Debug.Log($"[DialogueManager] DisplayFinalQuestText: {text}");
        EnableBubble();

        IsTextFullyVisible = false;
        _onTypingCompleted = null;
        _onTextClicked = onTextClicked;
        _isFinalQuestText = true;

        _typewriter.StartTyping(text);
    }

    public void DisplayTextWithCallback(string text, Action onComplete)
    {
        Debug.Log($"[DialogueManager] DisplayTextWithCallback called with text: {text}");   
        DisplayTextWithClickCallback(text, onComplete);
    }

    public void SetDialogueOptions(List<DialogueOption> options)
    {
        Debug.Log($"[DialogueManager] Setting dialogue options. Count: {(options != null ? options.Count : 0)}");
        
        _dialogueOptions.Clear();
        
        if (options != null)
        {
            foreach (var option in options)
            {
                _dialogueOptions.Add(new DialogueOptionData(option, true));
            }
        }
        
        RefreshDialogueHandles();
    }

    public void DeactivateDialogueOption(DialogueOption option)
    {
        Debug.Log($"[DialogueManager] Deactivating dialogue option: {option.TextKey}");
        
        var optionData = _dialogueOptions.FirstOrDefault(od => od.Option.Equals(option));
        if (optionData != null)
        {
            optionData.IsActive = false;
            RefreshDialogueHandles();
        }
    }
    
    public void ActivateDialogueOption(DialogueOption option)
    {
        Debug.Log($"[DialogueManager] Activating dialogue option: {option.TextKey}");
        
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
                    handle.Setup(customerManager.CurrentCustomer, optionData.Option);
            }
        }
    }
}