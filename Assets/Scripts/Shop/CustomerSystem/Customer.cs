using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private bool _canAcceptProduct = true;
    
    [SerializeField] protected CustomerQuest _quest;
    
    protected DialogueManager _dialogueManager;
    protected CustomerAnimatorController _animatorController;
    
    public event Action QuestCompleted;
    public event Action QuestInitialized;
    public event Action<GameObject> ProductAccepted;
    public event Action CounterReached;

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
        CounterReached?.Invoke();
        Initialize();
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    public virtual void FinishQuest()
    {
        QuestCompleted?.Invoke();
    }

    public void PlayOutDialogue(DialogueOption option)
    {
        option.Interaction.PlayOut(this, ReturnToQuestInteraction);
        //DisableReception();
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
        _canAcceptProduct = true;
    }

    public void DisableReception()
    {
        _canAcceptProduct = false;
    }

    public bool OfferProduct(Product product, BakeState bakeState, GameObject productObj)
    {
        if (!_canAcceptProduct) return false;

        bool successful = _quest.OfferProduct(product, bakeState);

        if (successful)
            ProductAccepted?.Invoke(productObj);

        return successful;
    }

    protected void Initialize()
    {
        if(_quest == null)
        {
            Debug.LogError("[Customer] No quest assigned to customer.");
            QuestInitialized?.Invoke();
            return;
        }
        _quest.Initialize(this);
        Debug.Log($"[Customer] Initialized Quest");
        QuestInitialized?.Invoke();
    }
}