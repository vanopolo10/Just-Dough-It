using UnityEngine;
using UnityEngine.UI;

public class SleepButton : MonoBehaviour
{
    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private TutorialRunner _tutorialRunner;
    
    private Button _button;
    private bool _isTutorial = false;

    private void Awake()
    {
        if (_customerManager == null)
            _customerManager = FindAnyObjectByType<CustomerManager>();
        
        if (_tutorialRunner == null)
            _tutorialRunner = FindAnyObjectByType<TutorialRunner>();
    }

    private void Start()
    {
        _button.interactable = false;
    }
    
    private void OnEnable()
    {
        _button = GetComponent<Button>();
        
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

    public void SetTutorial(bool isTutorial)
    {
        _isTutorial = isTutorial;
    }
    
    public void OnCustomersEnded()
    {
        _button.interactable = true;
    }

    private void OnSleep()
    {
        _button.interactable = false;
        
        if (_isTutorial)
        {
            SceneLoader.Instance.LoadScene(2);
            return;
        }

        _customerManager.EndDay();
    }
}