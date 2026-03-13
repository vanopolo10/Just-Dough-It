using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] protected float _timeoutBeforeInitializing = 4f;
    [SerializeField] protected float _timeoutBeforeResettingDialogue = 10f;
    
    [SerializeField] protected CustomerQuest _quest;
    
    protected DialogueManager _dialogueManager;
    protected CustomerAnimatorController _animatorController;

    public event Action QuestCompleted;
    
    public DialogueManager DialogueManager => _dialogueManager;
    public CustomerAnimatorController AnimatorController => _animatorController;

    protected void Start()
    {
        _dialogueManager = GetComponentInParent<DialogueManager>();
        _animatorController = GetComponentInChildren<CustomerAnimatorController>();
    }
    
    public void OnReachedCounter()
    {
        Invoke(nameof(Initialize), _timeoutBeforeInitializing);
    }
    
    public void Despawn()
    {
        if (_dialogueManager != null)
            _dialogueManager.Typewriter.TextDisplayed -= OnTextDisplayed;
        
        Destroy(gameObject);
    }

    public void StartQuest()
    {
        _quest.StartQuest();
    }

    public void FinishQuest()
    {
        CancelInvoke(nameof(ResetDialogue));
        QuestCompleted?.Invoke();
    }

    public void PlayOutDialogue(DialogueOption option)
    {
        CancelInvoke(nameof(ResetDialogue));

        if (_dialogueManager != null)
        {
            _dialogueManager.Typewriter.TextDisplayed += OnTextDisplayed;
        }

        option.Interaction.PlayOut(this);
    }

    private void OnTextDisplayed()
    {
        if (_dialogueManager)
        {
            _dialogueManager.Typewriter.TextDisplayed -= OnTextDisplayed;
        }

        Invoke(nameof(ResetDialogue), _timeoutBeforeResettingDialogue);
    }

    public void ResetDialogue()
    {
        CancelInvoke(nameof(ResetDialogue));

        _quest.QuestInteraction.PlayOut(this);
    }

    public bool OfferProduct(Product product)
    {
        CancelInvoke(nameof(ResetDialogue));
        Invoke(nameof(ResetDialogue), _timeoutBeforeResettingDialogue);

        bool successful = _quest.OfferProduct(product);

        return successful;
    }
    
    protected void Initialize()
    {
        _quest.Initialize(this);
    }

    private void OnDestroy()
    {
        if (_dialogueManager != null)
            _dialogueManager.Typewriter.TextDisplayed -= OnTextDisplayed;
        
        //if (_quest != null)
            //_quest.ResetQuest();
    }
}