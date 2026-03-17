using UnityEngine;
using System.Collections.Generic;

public class TutorialScenario : MonoBehaviour
{
    [SerializeField] private TutorialRunner _runner;

    [SerializeField] private CameraController _camera;
    [SerializeField] private DoughBucket _dough;
    [SerializeField] private PlayerThoughts _thoughts;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private RecipeManager _recipeManager;
    [SerializeField] private CustomerManager _customerManager;

    [Header("Icons")] 
    [SerializeField] private GameObject _nextDialogueIcon;
    [SerializeField] private GameObject _leftRightIcons;
    [SerializeField] private GameObject _rollingPinGuide;

    private Customer _customer;
    
    private void OnEnable()
    {
        if (_customerManager != null)
            _customerManager.CustomerSpawned += OnCustomerSpawned;
    }
    
    private void OnDisable()
    {
        if (_customerManager != null)
            _customerManager.CustomerSpawned -= OnCustomerSpawned;
    }

    private void OnCustomerSpawned(Customer customer)
    {
        _customer = customer;
        _customer.CustomerQuest.GreetingTypingCompleted += StartTutorialScenario;
    }

    private void StartTutorialScenario()
    {
        _customer.CustomerQuest.GreetingTypingCompleted -= StartTutorialScenario;
        
        _runner.StartTutorial(new List<ITutorialGate>
        {
            new ActionGate(() => _camera.BlockControl()),
            
            new ThoughtGate(_thoughts, "tutorial.think.start"),
            
            new DialogueGate(_dialogueManager, true ,_nextDialogueIcon),
            
            new DialogueGate(_dialogueManager, false),
            
            new ThoughtGate(_thoughts, "tutorial.think.sure"),
            
            new ActionGate(() => _camera.UnblockControl()),
            
            new CameraViewGate(_camera, CameraController.CameraViewType.Craft, _leftRightIcons),

            new ThoughtGate(_thoughts, "tutorial.think.remember"),

            new DoughStateGate(_dough, DoughState.Flat, _rollingPinGuide),

            new ThoughtGate(_thoughts, "tutorial.think.remember2"),
            
            new RecipeGate(_recipeManager, ProductType.SimplePie),
            
            new DoughStateGate(_dough, DoughState.FlatFolded),

            new ThoughtGate(_thoughts, "tutorial_make_pie"),

            new DoughStateGate(_dough, DoughState.SimplePie)
        });
    }
}