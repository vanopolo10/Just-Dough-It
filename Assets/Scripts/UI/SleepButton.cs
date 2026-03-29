using UnityEngine;
using UnityEngine.UI;

public class SleepButton : MonoBehaviour
{
    [SerializeField] private bool _isTutorial;
    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _customerManager.DayEnded += OnDayEnded;
        _button.onClick.AddListener(OnSleep);
    }
    
    private void OnDisable()
    {
        _customerManager.DayEnded -= OnDayEnded;
        _button.onClick.RemoveListener(OnSleep);
    }

    private void OnDayEnded()
    {
        _button.gameObject.SetActive(true);
    }

    private void OnSleep()
    {
        if (_isTutorial)
        {
            _button.gameObject.SetActive(false);
            SceneLoader.Instance.LoadScene(2);
            return;
        }

        _customerManager.StartNewDay();
    }
}
