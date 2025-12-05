using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedString : MonoBehaviour
{
    private TMP_Text _text;
    [SerializeField] private string _key;

    private void OnValidate()
    {
        _text = GetComponent<TMP_Text>();
        if (_text == null) Destroy(this);
    }

    private void UpdateText()
    {
        string Text = LocalizationManager.Instance.SelectedTable.GetPair(_key);
        if (Text == null) return;
        _text.text = Text;
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
}
