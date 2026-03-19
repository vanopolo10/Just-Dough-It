using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] protected CustomerQuest _quest;
    
    protected DialogueManager _dialogueManager;
    protected CustomerAnimatorController _animatorController;

    public event Action OnQuestCompleted;
    public event Action OnQuestInitialized;
    public event Action<GameObject> OnProductAccepted;

    public CustomerQuest Quest => _quest;
    public DialogueManager DialogueManager => _dialogueManager;
    public CustomerAnimatorController AnimatorController => _animatorController;
    private bool _canAcceptProduct = true;

    protected virtual void Start()
    {
        _dialogueManager = GetComponentInParent<DialogueManager>();
        _animatorController = GetComponentInChildren<CustomerAnimatorController>();
        DisableReception();
    }
    
    public void OnReachedCounter()
    {
        Initialize();
    }
    
    public void Despawn()
    {
        Destroy(gameObject);
    }
    
    public void FinishQuest()
    {
        OnQuestCompleted?.Invoke();
    }

    public void PlayOutDialogue(DialogueOption option)
    {
        option.Interaction.PlayOut(this, ReturnToQuestInteraction);
        DisableReception();
    }

    public void ReturnToQuestInteraction() {
        _quest.QuestInteraction.PlayOut(this);
        EnableReception();
    }

    public void EnableReception() {
        Debug.Log("[Customer] Reception enabled.");
        _canAcceptProduct = true;
    }

    public void DisableReception() {
        Debug.Log("[Customer] Reception disabled.");
        _canAcceptProduct = false;
    }
    public bool OfferProduct(Product product, GameObject productObj)
    {
        if(!_canAcceptProduct) return false;

        bool successful = _quest.OfferProduct(product);
        if (successful) { 
            OnProductAccepted?.Invoke(productObj);
        }
        return successful;
    }
    
    protected void Initialize()
    {
        _quest.Initialize(this);
        Debug.Log($"[Customer] Initialized Quest '{_quest.QuestInteraction.DialogueKey}' ");
        OnQuestInitialized?.Invoke();
    }
}