using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class CafeNameController : MonoBehaviour
{
    [SerializeField] private Button _submitButton;

    private TMP_InputField _input;

    public string CafeName { get; private set; }

    private void Awake()
    {
        _input = GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        _submitButton.gameObject.SetActive(false);
    }

    public void OnValueChanged(string value)
    {
        _submitButton.gameObject.SetActive(value != string.Empty);
        if (value.Length > 12) _input.text = value[..12];
        CafeName = value;
    }

    public void Clear()
    {
        _input.text = string.Empty;
    }
}
