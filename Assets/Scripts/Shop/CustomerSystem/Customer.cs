using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private bool _canAcceptProduct = true;
    
    [SerializeField] protected CustomerQuest _quest;
    
    protected DialogueManager _dialogueManager;
    protected CustomerAnimatorController _animatorController;
    
    public event Action OnQuestCompleted;
    public event Action OnQuestInitialized;
    public event Action<GameObject> OnProductAccepted;
    public event Action OnCounterReached;

    public CustomerQuest Quest => _quest;
    public DialogueManager DialogueManager => _dialogueManager;
    public CustomerAnimatorController AnimatorController => _animatorController;
    
    protected virtual void Start()
    {
        _dialogueManager = GetComponentInParent<DialogueManager>();
        _animatorController = GetComponentInChildren<CustomerAnimatorController>();
        DisableReception();
    }

    public void OnReachedCounter()
    {
        OnCounterReached?.Invoke();
        Initialize();
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    public virtual void FinishQuest()
    {
        OnQuestCompleted?.Invoke();
    }

    public void PlayOutDialogue(DialogueOption option)
    {
        option.Interaction.PlayOut(this, ReturnToQuestInteraction);
        DisableReception();
    }

    public void ReturnToQuestInteraction()
    {
        if (_quest != null && _quest.QuestInteraction != null)
        {
            _quest.QuestInteraction.PlayOut(this);
            EnableReception();
        }
    }

    public void EnableReception()
    {
        Debug.Log("[Customer] Reception enabled.");
        _canAcceptProduct = true;
    }

    public void DisableReception()
    {
        Debug.Log("[Customer] Reception disabled.");
        _canAcceptProduct = false;
    }

    public bool OfferProduct(Product product, GameObject productObj)
    {
        if (!_canAcceptProduct) return false;

        bool successful = _quest.OfferProduct(product);

        if (successful)
            OnProductAccepted?.Invoke(productObj);

        return successful;
    }

    protected void Initialize()
    {
        if(_quest == null)
        {
            Debug.LogError("[Customer] No quest assigned to customer.");
            OnQuestInitialized?.Invoke();
            return;
        }
        _quest.Initialize(this);
        Debug.Log($"[Customer] Initialized Quest");
        OnQuestInitialized?.Invoke();
    }
}