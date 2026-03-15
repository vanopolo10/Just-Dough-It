using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private TextTypewriter _typewriter;
    [SerializeField] private GameObject _baseDialogueOption, _baseInactiveDialogueOption;
    [SerializeField] private float _dialogueOptionOffset = 1f;
    
    private List<DialogueOption> _dialogueOptions;
    private List<bool> _dialogueOptionActive;
    private List<Transform> _spawnedDialogueOptions = new();

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

    public void DisplayText(string text, float delayAfterTyping = 0f)
    {
        if (!_speechBubble.activeSelf)
            EnableBubble();
        
        _typewriter.StartTyping(text, delayAfterTyping);
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
        _speechBubble.SetActive(true);
        RefreshDialogueHandles();
    }

    public void DisableBubble()
    {
        ClearDialogueHandles();
        _speechBubble.SetActive(false);
        _typewriter.Clear();
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
            if (handle)
            {
                var customerManager = GetComponent<CustomerManager>();
                if (customerManager)
                    handle.Setup(customerManager.CurrentCustomer, option);
            }
        }
    }

    public void Timeout(float time)
    {
        Invoke(nameof(DisableBubble), time);
    }

    private void OnDisable()
    {
        if (_typewriter != null)
            _typewriter.Clear();
    }
}