using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] protected float _timeoutBeforeInitializing = 4f,
        _timeoutBeforeDespawning = 6f,
        _timeoutBeforeResettingDialogue = 10f;

    [SerializeField] protected CustomerQuest _quest;
    
    protected CustomerManager _manager;
    protected DialogueManager _dialogueManager;
    protected Animator _animator;
    protected CustomerAnimatorController _animatorController;
    
    public DialogueManager DialogueManager => _dialogueManager;
    public Animator Animator => _animator;
    public CustomerAnimatorController AnimatorController => _animatorController;

    protected void Start()
    {
        _dialogueManager = GetComponentInParent<DialogueManager>();
        _animator = GetComponentInChildren<Animator>();
        _animatorController = GetComponentInChildren<CustomerAnimatorController>();
        _manager = GetComponentInParent<CustomerManager>();
    }
    
    public void OnReachedCounter()
    {
        Invoke(nameof(Initialize), _timeoutBeforeInitializing);
    }

    protected void Initialize()
    {
        _quest.Initialize(this);
    }

    protected void Despawn()
    {
        _manager.NextCustomer();
        Destroy(gameObject);
    }

    public void StartQuest()
    {
        _quest.StartQuest();
    }

    public void FinishQuest()
    {
        CancelInvoke(nameof(ResetDialogue));

        Invoke(nameof(Despawn), _timeoutBeforeDespawning);
    }

    public void PlayOutDialogue(DialogueOption option)
    {
        CancelInvoke(nameof(ResetDialogue));
        Invoke(nameof(ResetDialogue), _timeoutBeforeResettingDialogue);

        option.interaction.PlayOut(this);
    }

    public void ResetDialogue()
    {
        CancelInvoke(nameof(ResetDialogue));

        _quest.questInteraction.PlayOut(this);
    }

    public bool OfferProduct(Product product)
    {
        CancelInvoke(nameof(ResetDialogue));
        Invoke(nameof(ResetDialogue), _timeoutBeforeResettingDialogue);

        bool successful = _quest.OfferProduct(product);

        return successful;
    }
}