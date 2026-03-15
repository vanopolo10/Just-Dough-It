using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private bool _isTutorial;
    
    [SerializeField] private DialogueBubble _speechBubble;
    [SerializeField] private TextTypewriter _typewriter;
    [SerializeField] private GameObject _baseDialogueOption, _baseInactiveDialogueOption;
    [SerializeField] private float _dialogueOptionOffset = 1f;
    
    private List<DialogueOption> _dialogueOptions;
    private List<bool> _dialogueOptionActive;
    private List<Transform> _spawnedDialogueOptions = new();

    private Action _onCompleteCurrentText;
    private bool _isFinalQuestText;

    public event Action SkipClicked;
    public event Action ConfirmClicked;
    public event Action OnGreetingCompleted;
    
    public bool IsTextFullyVisible { get; private set; }
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
        if (_typewriter.IsTyping)
        {
            _typewriter.CompleteTypingInstantly();
            IsTextFullyVisible = true;

            SkipClicked?.Invoke();
            return;
        }

        if (IsTextFullyVisible)
        {
            ConfirmClicked?.Invoke();

            var callback = _onCompleteCurrentText;
            bool wasFinalText = _isFinalQuestText;

            IsTextFullyVisible = false;
            _onCompleteCurrentText = null;
            _isFinalQuestText = false;

            if (callback != null)
            {
                if (wasFinalText)
                    SetDialogueOptions(new List<DialogueOption>());

                callback.Invoke();
            }
        }
    }

    private void OnTypingCompleted()
    {
        if (_isTutorial)
        {
            OnGreetingCompleted?.Invoke();
        }

        IsTextFullyVisible = true;
        _typewriter.TypingCompleted -= OnTypingCompleted;
    }

    public void DisplayText(string text)
    {
        if (!_speechBubble.gameObject.activeSelf)
            EnableBubble();
        
        IsTextFullyVisible = false;
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
        
        IsTextFullyVisible = false;
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
        IsTextFullyVisible = false;
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