using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject _speechBubble;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = _speechBubble.GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Start()
    {
        DisableBubble();
        
    }

    private void EnableBubble() {
        _speechBubble.SetActive(true);
    }

    private void DisableBubble()
    {
        _speechBubble.SetActive(false);
    }

    public void DisplayText(string text) { // replace with localization key later 
        _text.text = text;
        if(_speechBubble.activeSelf == false) EnableBubble();
    }

    public void Timeout(float time) { 
        Invoke(nameof(DisableBubble), time);
    }
}
