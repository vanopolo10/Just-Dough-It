using UnityEngine;
using UnityEngine.UI;

public class SleepButton : MonoBehaviour
{
    [SerializeField] private bool _isTutorial;
    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private TutorialRunner _tutorialRunner;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_isTutorial && _tutorialRunner != null)
            _tutorialRunner.TutorialComplited += OnCustomersEnded;
        else
            _customerManager.CustomersEnded += OnCustomersEnded;
        
        _button.onClick.AddListener(OnSleep);
    }
    
    private void OnDisable()
    {
        if (_isTutorial && _tutorialRunner != null)
            _tutorialRunner.TutorialComplited -= OnCustomersEnded;
        else
            _customerManager.CustomersEnded -= OnCustomersEnded;
        
        _button.onClick.RemoveListener(OnSleep);
    }

    private void OnCustomersEnded()
    {
        _button.gameObject.SetActive(true);
    }

    private void OnSleep()
    {
        _button.gameObject.SetActive(false);
        
        if (_isTutorial)
        {
            SceneLoader.Instance.LoadScene(2);
            return;
        }

        _customerManager.EndDay();
    }
}
