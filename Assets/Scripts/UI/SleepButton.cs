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
        _button = GetComponent<Button>();
        _button.interactable = false;

        if (_customerManager == null)
            _customerManager = FindAnyObjectByType<CustomerManager>();
        
        if (_tutorialRunner == null)
            _tutorialRunner = FindAnyObjectByType<TutorialRunner>();
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
        if (_isTutorial)
        {
            Debug.Log($"SceneLoader.Instance is null: {SceneLoader.Instance == null}");

            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(2);
            }
            else
            {
                Debug.LogError("SceneLoader.Instance is null! Loading directly.");
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            }
            return;
        }
        
        _button.interactable = false;
        _customerManager.EndDay();
    }
}