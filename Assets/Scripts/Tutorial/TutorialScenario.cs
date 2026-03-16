using UnityEngine;
using System.Collections.Generic;

public class TutorialScenario : MonoBehaviour
{
    [SerializeField] private TutorialRunner _runner;

    [SerializeField] private CameraController _camera;
    [SerializeField] private DoughBucket _dough;
    [SerializeField] private PlayerThoughts _thoughts;
    [SerializeField] private DialogueManager _dialogueManager;

    [Header("Icons")] 
    [SerializeField] private GameObject _nextDialogueIcon;
    
    private void Start()
    {
        _runner.StartTutorial(new List<ITutorialGate>
        {
            new ThoughtGate(_thoughts, "tutorial_turn_to_craft"),
            
            new DialogueGate(_dialogueManager, _nextDialogueIcon),
            
            new ActionGate(() => _camera.UnblockControl()),
            
            new CameraViewGate(_camera, CameraController.CameraViewType.Craft, _nextDialogueIcon),

            new ThoughtGate(_thoughts, "tutorial_roll_dough"),

            new DoughStateGate(_dough, DoughState.Flat),

            new ThoughtGate(_thoughts, "tutorial_fold_dough"),

            new DoughStateGate(_dough, DoughState.FlatFolded),

            new ThoughtGate(_thoughts, "tutorial_make_pie"),

            new DoughStateGate(_dough, DoughState.SimplePie)
        });
    }
}