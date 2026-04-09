using UnityEngine;
using UnityEngine.UI;

public class SleepButton : MonoBehaviour
{
    [SerializeField] private bool _isTutorial;
    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private TutorialRunner _tutorialRunner;
    
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.gameObject.SetActive(false);
        
        if (_customerManager == null)
            _customerManager = FindAnyObjectByType<CustomerManager>();
        
        if (_tutorialRunner == null)
            _tutorialRunner = FindAnyObjectByType<TutorialRunner>();
    }

    private void OnEnable()
    {
        if (_isTutorial && _tutorialRunner != null)
            _tutorialRunner.TutorialComplited += OnCustomersEnded;
        else if (_customerManager != null)
            _customerManager.CustomersEnded += OnCustomersEnded;
        
        _button.onClick.AddListener(OnSleep);
    }
    
    private void OnDisable()
    {
        if (_isTutorial && _tutorialRunner != null)
            _tutorialRunner.TutorialComplited -= OnCustomersEnded;
        else if (_customerManager != null)
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