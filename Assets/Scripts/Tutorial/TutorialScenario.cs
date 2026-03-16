using UnityEngine;
using UnityEngine.UI;

public class TutorialScenario : MonoBehaviour
{
    [Header("Information")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private DoughBucket _doughBucket;

    [Header("Icons")] 
    [SerializeField] private PlayerThoughts _playerThoughts;
    [SerializeField] private Image _dialogueCursor;

    private enum TutorialState
    {
        WaitingForGreetingFinish,
        WaitingForConfirmClick,
        WaitingForCraftView,
        WaitingForFlat,
        WaitingForFolded,
        WaitingForPie,
        Completed
    }

    private TutorialState _state = TutorialState.WaitingForGreetingFinish;

    private void OnEnable()
    {
        _dialogueManager.SkipClicked += OnSkipClicked;
        _dialogueManager.ConfirmClicked += OnConfirmClicked;
        _dialogueManager.OnGreetingCompleted += OnGreetingCompleted;

        _cameraController.ViewChanged += OnViewChanged;
        _doughBucket.DoughStateChanged += OnDoughStateChanged;
    }

    private void OnDisable()
    {
        _dialogueManager.SkipClicked -= OnSkipClicked;
        _dialogueManager.ConfirmClicked -= OnConfirmClicked;
        _dialogueManager.OnGreetingCompleted -= OnGreetingCompleted;

        _cameraController.ViewChanged -= OnViewChanged;
        _doughBucket.DoughStateChanged -= OnDoughStateChanged;
    }

    private void Start()
    {
        _cameraController.BlockControl();
        Debug.Log("[Tutorial] Movement BLOCKED");
    }

    private void OnGreetingCompleted()
    {
        if (_state != TutorialState.WaitingForGreetingFinish)
            return;

        _dialogueCursor.enabled = true;
        _state = TutorialState.WaitingForConfirmClick;
    }

    private void OnSkipClicked()
    {
        if (_state != TutorialState.WaitingForGreetingFinish) 
            return;
        
        _dialogueCursor.enabled = true;
        _state = TutorialState.WaitingForConfirmClick;
    }

    private void OnConfirmClicked()
    {
        if (_state != TutorialState.WaitingForConfirmClick)
            return;
        
        _dialogueCursor.enabled = false;

        Debug.Log("[Tutorial] Movement unlocked");

        _cameraController.UnblockControl();

        Debug.Log("[Tutorial] Turn RIGHT twice to reach Craft");

        _state = TutorialState.WaitingForCraftView;
    }

    private void OnViewChanged(CameraController.CameraViewType view)
    {
        if (_state != TutorialState.WaitingForCraftView)
            return;

        if (view == CameraController.CameraViewType.Craft)
        {
            Debug.Log("[Tutorial] Craft station reached");

            Debug.Log("[Tutorial] Roll the dough");

            _state = TutorialState.WaitingForFlat;
        }
    }

    private void OnDoughStateChanged(DoughState state)
    {
        if (_state == TutorialState.WaitingForFlat && state == DoughState.Flat)
        {
            Debug.Log("[Tutorial] Dough rolled ? now fold it");

            _state = TutorialState.WaitingForFolded;
            return;
        }

        if (_state == TutorialState.WaitingForFolded && state == DoughState.FlatFolded)
        {
            Debug.Log("[Tutorial] Dough folded ? make pie");

            _state = TutorialState.WaitingForPie;
            return;
        }

        if (_state == TutorialState.WaitingForPie && state == DoughState.SimplePie)
        {
            Debug.Log("[Tutorial] Pie created!");

            _state = TutorialState.Completed;

            Debug.Log("[Tutorial] Tutorial completed!");
        }
    }
}