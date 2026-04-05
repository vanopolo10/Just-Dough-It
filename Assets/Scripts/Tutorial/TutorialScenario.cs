using UnityEngine;
using System.Collections.Generic;

public class TutorialScenario : MonoBehaviour
{
    [SerializeField] private TutorialRunner _runner;

    [Header("Info")] 
    [SerializeField] private CameraController _camera;
    [SerializeField] private DoughBucket _doughBucket;
    [SerializeField] private PlayerThoughts _thoughts;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private Book _book;
    [SerializeField] private CustomerManager _customerManager;
    [SerializeField] private CustomerRouteMover _routeMover;
    [SerializeField] private RollingPin _rollingPin;
    [SerializeField] private OvenSender _ovenSender;
    [SerializeField] private Tray _tray;
    [SerializeField] private Oven _oven;
    [SerializeField] private Filling _jam;
    [SerializeField] private Filling _farce;
    [SerializeField] private Thermometer _thermometer;
    [SerializeField] private Hatch _hatch;
    
    [Header("Icons")] 
    [SerializeField] private GameObject _nextDialogueIcon;
    [SerializeField] private GameObject _dialogueOptionIcon;
    [SerializeField] private GameObject _leftRightIcons;
    [SerializeField] private GameObject _rightIcon;
    [SerializeField] private GameObject _leftIcon;
    [SerializeField] private GameObject _backIcon;
    [SerializeField] private GameObject _rollingPinGuide;
    [SerializeField] private GameObject _foldingGuide;
    [SerializeField] private GameObject _pressingGuide;
    [SerializeField] private GameObject _bookGuide;
    [SerializeField] private GameObject _toBowlGuide;
    [SerializeField] private GameObject _fromBowlGuide;
    [SerializeField] private GameObject _fillingGuide;
    [SerializeField] private GameObject _toOvenGuide;
    [SerializeField] private GameObject _trayClickIcon;
    [SerializeField] private GameObject _ovenClickIcon;
    [SerializeField] private GameObject _hatchClickIcon;
    [SerializeField] private GameObject _waitingGuide;
    [SerializeField] private GameObject _trayDoughIcon;
    [SerializeField] private GameObject _shelfDragIcon;

    private Customer _customer;
    private bool _tutorialStarted = false;

    private void Start()
    {
        if (_doughBucket.CurrentDough == null)
            _doughBucket.SpawnDough(DoughState.Raw, FillingType.None);

        _doughBucket.CurrentDough.SetCanDrag(false);
        
        _camera.SetControlBlock(false, false, false);
        _book.SetCanOpen(false);
        _jam.SetCanGrab(false);
        _farce.SetCanGrab(false);
        _ovenSender.SetCanSend(false);
        _tray.SetCanMove(false);
        _thermometer.gameObject.SetActive(false);
        _hatch.SetCanMove(false);
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
            new TalkGate(_dialogueManager, _dialogueOptionIcon, "tutorial.player.sure"),
            
            new ActionGate(() => _camera.SetControlBlock(true, true, false)),
            new CameraViewGate(_camera, CameraController.CameraViewType.Craft, _leftRightIcons),
            new ActionGate(() => _thermometer.gameObject.SetActive(true)),
            
            new ActionGate(() => _book.SetCanOpen(true)), 
            new ActionGate(() => _camera.SetControlBlock(false, false, false)),
            new ActionGate(() => _rollingPin.SetDragAllowed(false)),
            new RecipeGate(_book, ProductType.SimplePie, _bookGuide),
            new ActionGate(() => _book.SetCanOpen(false)),
            new ActionGate(() => _book.Disable()), 
            
            new ActionGate(() => _rollingPin.SetDragAllowed(true)),
            new ActionGate(() => _thoughts.Think("tutorial.think.rolling")),
            new DoughStateGate(_doughBucket, DoughState.Flat, _rollingPinGuide),
            new ActionGate(() => _rollingPin.SetDragAllowed(false)),
            new ActionGate(() => _rollingPin.MoveToStart()),
            
            new ActionGate(() => _thoughts.Think("tutorial.think.folding")),
            new DoughStateGate(_doughBucket, DoughState.FlatFolded, _foldingGuide),

            new ActionGate(() => _thoughts.Think("tutorial.think.pressing")),
            new DoughStateGate(_doughBucket, DoughState.SimplePie, _pressingGuide),
            
            new ActionGate(() => _doughBucket.CurrentDough.SetCanDrag(true)),
            new ActionGate(() => _thoughts.Think("tutorial.think.forgot")),
            new DoughRemovedGate(_doughBucket, _toBowlGuide),
            new DoughPutGate(_doughBucket, _fromBowlGuide),
            
            new ActionGate(() => _thoughts.Think("tutorial.think.fast")),
            new ActionGate(() => _rollingPin.SetDragAllowed(true)),
            new DoughStateGate(_doughBucket, DoughState.Flat),
            new ActionGate(() => _rollingPin.SetDragAllowed(false)),
            new ActionGate(() => _rollingPin.MoveToStart()),
            
            new ActionGate(() => _doughBucket.CurrentDough.SetCanActing(false)),
            new ActionGate(() => _thoughts.Think("tutorial.think.filling")),
            new ActionGate(() => _jam.SetCanGrab(true)),
            new ActionGate(() => _farce.SetCanGrab(true)),
            new PutFillingGate(_doughBucket, _fillingGuide),
            
            new ActionGate(() => _doughBucket.CurrentDough.SetCanActing(true)),
            new DoughStateGate(_doughBucket, DoughState.SimplePie),
            
            new ActionGate(() => _ovenSender.SetCanSend(true)),
            new ActionGate(() => _thoughts.Close()),
            new DoughSendGate(_ovenSender, _toOvenGuide),
            
            new ActionGate(() => _camera.SetControlBlock(false, true, false)),
            new CameraViewGate(_camera, CameraController.CameraViewType.Oven, _rightIcon),
            new ActionGate(() => _camera.SetControlBlock(false, false, false)),
            new ActionGate(() => _tray.SetCanMove(true)),
            new TrayGate(_tray, true, _trayClickIcon),
            new ActionGate(() => _tray.SetCanMove(false)),
            
            new ActionGate(() => _camera.SetControlBlock(false, true, false)),
            new CameraViewGate(_camera, CameraController.CameraViewType.OvenDown, _rightIcon),
            new ActionGate(() => _camera.SetControlBlock(false, false, false)),
            new ActionGate(() => _tray.StopBake()),
            
            new ActionGate(() => _hatch.SetCanMove(true)),
            new HatchGate(_hatch, _hatchClickIcon),
            new ActionGate(() => _hatch.SetCanMove(false)),
            
            new ActionGate(() => _thermometer.SetCanAddWood(true)),
            new ActionGate(() => _thoughts.Think("tutorial.think.wood")),
            new OvenGate(_oven,false, 0, _ovenClickIcon),
            new ActionGate(() => _thermometer.SetCanAddWood(false)),
            new ActionGate(() => _thoughts.Think("tutorial.think.wood2")),
            new OvenGate(_oven, true, 12),
            
            new ActionGate(() => _hatch.SetCanMove(true)),
            new ActionGate(() => _thoughts.Think("tutorial.think.hatch")),
            new HatchGate(_hatch),
            new ActionGate(() => _hatch.SetCanMove(false)),
            
            new ActionGate(() => _camera.SetControlBlock(true, false, false)), 
            new CameraViewGate(_camera, CameraController.CameraViewType.Oven, _leftIcon),
            new ActionGate(() => _tray.StartBake()),
            new ActionGate(() => _camera.SetControlBlock(false, false, false)), 
            new BakeGate(_tray, BakeState.Done, _waitingGuide),
            new ActionGate(() => _tray.StopBake()),
            new ActionGate(() => _thoughts.Think("tutorial.think.done")),
            
            new ActionGate(() => _tray.SetCanMove(true)),
            new TrayGate(_tray, true, _trayClickIcon),
            new ActionGate(() => _tray.SetCanMove(false)),
            new TrayGate(_tray, false, _trayDoughIcon),
            new ActionGate(() => _thoughts.Close()),
            
            new ActionGate(() => _camera.SetControlBlock(false, false, true)),
            new CameraViewGate(_camera, CameraController.CameraViewType.Door, _backIcon),
            new ActionGate(() => _camera.SetControlBlock(false, false, false)),
            new ItemAcceptGate(_customer, _shelfDragIcon),
            new CustomerLeftGate(_routeMover),
            new ThoughtGate(_thoughts, "tutorial.think.open")
        });
    }
}