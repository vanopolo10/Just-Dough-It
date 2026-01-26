using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] protected float _timeoutBeforeInitializing = 4f, _timeoutBeforeDespawning = 6f;
    [SerializeField] protected CustomerQuest _quest;
    protected CustomerManager _manager;
    protected DialogueManager _dialogueManager;
    protected Animator _animator;
    public DialogueManager DialogueManager => _dialogueManager;
    public Animator Animator => _animator;

    protected void Initialize() {
        _quest.Initialize(this);
    }
    protected void Start()
    {
        _dialogueManager = GetComponentInParent<DialogueManager>();
        _animator = GetComponentInChildren<Animator>();
        _manager = GetComponentInParent<CustomerManager>();

        Invoke(nameof(Initialize), _timeoutBeforeInitializing);
    }

    protected void Despawn() { 
        _manager.StartNewCycle();
        Destroy(gameObject);
    }
    public void StartQuest() { 
        _quest.StartQuest();
    }
    public void FinishQuest() { 
        Invoke(nameof(Despawn), _timeoutBeforeDespawning);
    }
    public void PlayOutDialogue(DialogueOption option)
    {
        option.interaction.PlayOut(this);
    }


    public bool OfferProduct(Product product) { 
        bool successful = _quest.OfferProduct(product);

        return successful;
    }
}
