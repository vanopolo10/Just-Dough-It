using UnityEngine;
using System.Collections.Generic;

public class TutorialScenario : MonoBehaviour
{
    [SerializeField] private TutorialRunner _runner;

    [SerializeField] private CameraController _camera;
    [SerializeField] private DoughBucket _dough;
    [SerializeField] private PlayerThoughts _thoughts;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private Book _book;
    [SerializeField] private CustomerManager _customerManager;

    [Header("Icons")] 
    [SerializeField] private GameObject _nextDialogueIcon;
    [SerializeField] private GameObject _leftRightIcons;
    [SerializeField] private GameObject _rollingPinGuide;
    [SerializeField] private GameObject _bookGuide;

    private Customer _customer;
    private bool _tutorialStarted = false;

    private void Start()
    {
        _dough.CurrentDough.GetComponent<DoughDrag>().Block();
        _camera.BlockControl();
        _book.Block();
    }

    private void OnEnable()
    {
        if (_customerManager != null)
            _customerManager.CustomerSpawned += OnCustomerSpawned;
    }
    
    private void OnDisable()
    {
        if (_customerManager != null)
            _customerManager.CustomerSpawned -= OnCustomerSpawned;
            
        if (_customer != null && _customer.Quest != null)
            _customer.Quest.GreetingTypingCompleted -= StartTutorialScenario;
    }

    private void OnCustomerSpawned(Customer customer)
    {
        _customer = customer;

        if (_customer != null && _customer.Quest != null)
            _customer.Quest.GreetingTypingCompleted += StartTutorialScenario;
    }

    private void StartTutorialScenario()
    {
        if (_tutorialStarted) return;
        _tutorialStarted = true;
        
        if (_customer != null && _customer.Quest != null)
            _customer.Quest.GreetingTypingCompleted -= StartTutorialScenario;
        
        _runner.StartTutorial(new List<ITutorialGate>
        {
            new ActionGate(() => _thoughts.Think("tutorial.think.start", true)),
            
            new DialogueGate(_dialogueManager, true, _nextDialogueIcon),
            new DialogueGate(_dialogueManager, false),
            
            new ActionGate(() => _thoughts.Think("tutorial.think.sure")),
            new ActionGate(() => _camera.UnblockControl()),
            new CameraViewGate(_camera, CameraController.CameraViewType.Craft, _leftRightIcons),
            
            new ActionGate(() => _book.Unblock()),
            new RecipeGate(_book, ProductType.SimplePie, _bookGuide),
            new ActionGate(() => _book.Block()),
            new ActionGate(() => _book.Disable()),
            
            new ActionGate(() => _thoughts.Think("tutorial.think.rolling")),
            new DoughStateGate(_dough, DoughState.Flat, _rollingPinGuide),
            
            new ActionGate(() => _thoughts.Think("tutorial.think.folding")),
            new DoughStateGate(_dough, DoughState.FlatFolded),

            new ActionGate(() => _thoughts.Think("tutorial.think.pressing")),
            new DoughStateGate(_dough, DoughState.SimplePie),
            new ActionGate(_dough.CurrentDough.GetComponent<DoughDrag>().Unblock)
        });
    }
}