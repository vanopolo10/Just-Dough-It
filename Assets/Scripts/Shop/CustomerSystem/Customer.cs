using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
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
        Initialize();
    }
    
    public void Despawn()
    {
        Destroy(gameObject);
    }

    public void FinishQuest()
    {
        Debug.Log($"[Customer] FinishQuest called for: {gameObject.name}");
        QuestCompleted?.Invoke();
    }

    public void PlayOutDialogue(DialogueOption option)
    {
        option.Interaction.PlayOut(this);
    }

    public bool OfferProduct(Product product)
    {
        bool successful = _quest.OfferProduct(product);
        return successful;
    }
    
    protected void Initialize()
    {
        _quest.Initialize(this);
    }
}