using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private GameObject _baseDialogueOption, _baseInactiveDialogueOption;
    [SerializeField] private float _dialogueOptionOffset = 1f;
    private List<DialogueOption> _dialogueOptions;
    private List<bool> _dialogueOptionActive;
    private List<Transform> _spawnedDialogueOptions = new List<Transform>();
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = _speechBubble.GetComponentInChildren<TextMeshProUGUI>();
        SetDialogueOptions(new List<DialogueOption>());
    }
    private void Start()
    {
        DisableBubble();
    }

    private void EnableBubble() {
        _speechBubble.SetActive(true);
        RefreshDialogueHandles();
    }

    private void DisableBubble()
    {
        ClearDialogueHandles();
        _speechBubble.SetActive(false);
    }

    public void DisplayText(string text) { // replace with localization key later 
        _text.text = text;
        if(_speechBubble.activeSelf == false) EnableBubble();
    }
    public void SetDialogueOptions(List<DialogueOption> options) {
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
    public void ClearDialogueHandles() {
        if(_spawnedDialogueOptions == null) return;
        foreach (Transform child in _spawnedDialogueOptions)
        {
            if (child.gameObject != _baseDialogueOption && child.gameObject != _baseInactiveDialogueOption)
            {
                Destroy(child.gameObject);
            }
        }
        _spawnedDialogueOptions = new List<Transform>();
    }

    public void RefreshDialogueHandles() {
        ClearDialogueHandles();

        Vector3 curOffset = Vector3.zero;
        foreach (DialogueOption option in _dialogueOptions)
        {
            GameObject optionObj;
            if(_dialogueOptionActive[_dialogueOptions.IndexOf(option)] == true)
                optionObj = Instantiate(_baseDialogueOption, _baseDialogueOption.transform.parent);
            else 
                optionObj = Instantiate(_baseInactiveDialogueOption, _baseInactiveDialogueOption.transform.parent);
            optionObj.SetActive(true);
            _spawnedDialogueOptions.Add(optionObj.transform);

            optionObj.transform.localPosition += curOffset;
            curOffset.y -= _dialogueOptionOffset;

            optionObj.GetComponent<DialogueOptionHandle>().Setup(GetComponent<CustomerManager>().CurrentCustomer, option);
        }
    }

    public void Timeout(float time) { 
        Invoke(nameof(DisableBubble), time);
    }
}
