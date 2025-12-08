using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedString : MonoBehaviour
{
    [SerializeField, HideInInspector] private TMP_Text _text;
    [SerializeField] private string _key;

    private void OnValidate()
    {
        _text = GetComponent<TMP_Text>();
        if (_text == null) Destroy(this);
    }

    private void UpdateText()
    {
        string text = LocalizationManager.Instance.SelectedTable.GetPair(_key);
        
        if (text == null) return;
        _text.text = text;
    }

    private void Start()
    {
        if (LocalizationManager.Instance == null) return;
        LocalizationManager.Instance.OnLanguageChange += UpdateText;
        UpdateText();
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance == null) return;
        LocalizationManager.Instance.OnLanguageChange += UpdateText;
        UpdateText();
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance == null) return;
        LocalizationManager.Instance.OnLanguageChange -= UpdateText;
    }

    public void SetKey(string key)
    {
        _key = key;
        UpdateText();
    }

}
