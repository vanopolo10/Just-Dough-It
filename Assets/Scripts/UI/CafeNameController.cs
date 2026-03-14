using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class CafeNameController : MonoBehaviour
{
    [SerializeField] private Button _submitButton;

    public string CafeName { get; private set; }

    private void Start()
    {
        _submitButton.gameObject.SetActive(false);
    }

    public void OnValueChanged(string value)
    {
        _submitButton.gameObject.SetActive(value != string.Empty);

        CafeName = value;
    }
}
