using UnityEngine;
using System.Collections.Generic;

public class TutorialScenario : MonoBehaviour
{
    [SerializeField] private TutorialRunner _runner;

    [Header("Info")] 
    [SerializeField] private CameraController _camera;
    [SerializeField] private DoughBucket _dough;
    [SerializeField] private PlayerThoughts _thoughts;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private Book _book;
    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private RollingPin _rollingPin;
    [SerializeField] private OvenSender _ovenSender;
    [SerializeField] private Tray _tray;
    [SerializeField] private Oven _oven;
    
    [Header("Icons")] 
    [SerializeField] private GameObject _nextDialogueIcon;
    [SerializeField] private GameObject _dialogueOptionIcon;
    [SerializeField] private GameObject _leftRightIcons;
    [SerializeField] private GameObject _rightIcon;
    [SerializeField] private GameObject _rollingPinGuide;
    [SerializeField] private GameObject _bookGuide;
    [SerializeField] private GameObject _toOvenGuide;
    [SerializeField] private GameObject _trayClickIcon;

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
            new TalkGate(_dialogueManager, _dialogueOptionIcon, "tutorial.think.sure"),
            
            new ActionGate(() => _camera.UnblockControl()),
            new CameraViewGate(_camera, CameraController.CameraViewType.Craft, _leftRightIcons),
            
            new ActionGate(() => _book.Unblock()), 
            new ActionGate(() => _camera.BlockControl()),
            new ActionGate(() => _rollingPin.Block()),
            new RecipeGate(_book, ProductType.SimplePie, _bookGuide),
            new ActionGate(() => _book.Block()),
            new ActionGate(() => _book.Disable()), 
            
            new ActionGate(() => _rollingPin.Unblock()),
            new ActionGate(() => _thoughts.Think("tutorial.think.rolling")),
            new DoughStateGate(_dough, DoughState.Flat, _rollingPinGuide),
            new ActionGate(() => _rollingPin.Block()),
            
            new ActionGate(() => _thoughts.Think("tutorial.think.folding")),
            new DoughStateGate(_dough, DoughState.FlatFolded),

            new ActionGate(() => _thoughts.Think("tutorial.think.pressing")),
            new DoughStateGate(_dough, DoughState.SimplePie),
            
            new ActionGate(_dough.CurrentDough.GetComponent<DoughDrag>().Unblock),
            new ActionGate(() => _thoughts.Close()),
            new DoughSendGate(_ovenSender, _toOvenGuide),
            
            new CameraViewGate(_camera, CameraController.CameraViewType.Oven, _rightIcon),
            new TrayGate(_tray, true, _trayClickIcon),
            
            new CameraViewGate(_camera, CameraController.CameraViewType.OvenDown, _rightIcon),
            new OvenGate(_oven)
        });
    }
}