using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Product
{
    public Product(ProductType type, FillingType filling)
    {
        Type = type;
        Filling = filling;
    }

    public ProductType Type { get; set; }
    public FillingType Filling { get; set; }
}

[Serializable, CreateAssetMenu(fileName = "CustomerQuest", menuName = "ScriptableObjects/CustomerSystem/CustomerQuest")]
public class CustomerQuest : ScriptableObject
{
    [SerializeField] private CustomerInteractionSet _interactions;
    [SerializeField] private CustomerInteraction _questInteraction;
    [SerializeField] private CustomerInteractionSequence _customDecline;
    [SerializeField] private int _productsNeeded;
    [SerializeField] private List<ProductType> _applicableTypes;
    [SerializeField] private List<FillingType> _applicableFillings;
    [SerializeField] private bool _playReactionSounds = true;
    //[SerializeField] private List<DialogueOption> _overrideDialogueOptions;

    private int _productsLeft;
    private Customer _customer;
    private bool _isInitialized;

    public event Action GreetingTypingCompleted;
    public event Action QuestStarted;

    public CustomerInteractionSet Interactions => _interactions;
    public CustomerInteraction QuestInteraction => _questInteraction;
    public int ProductsLeft => _productsLeft;

    public void Initialize(Customer customer)
    {
        if (customer == null)
        {
            Debug.LogError($"CustomerQuest {name}: Customer is null");
            return;
        }

        _customer = customer;
        _productsLeft = _productsNeeded;
        _isInitialized = true;

        if (_customer.AnimatorController != null)
            _customer.AnimatorController.OnGreeting();

        if (_interactions != null && _interactions.OnGreeting != null)
        {
            _customer.DialogueManager.TypingCompleted += OnGreetingTypingCompleted;
            _interactions.OnGreeting.PlayOut(_customer, StartQuest);
        }
        else
        {
            Debug.LogWarning($"CustomerQuest {name}: OnGreeting is missing. Starting quest immediately.");
            StartQuest();
        }
        /*
        if (_overrideDialogueOptions.Count != 0) { 
            _customer.DialogueManager.SetDialogueOptions(_overrideDialogueOptions);
        }
        */
    }


    private void OnGreetingTypingCompleted()
    {
        Debug.Log("[CustomerQuest] GreetingTypingCompleted fired!");
        GreetingTypingCompleted?.Invoke();

        if (_customer != null && _customer.DialogueManager != null)
            _customer.DialogueManager.TypingCompleted -= OnGreetingTypingCompleted;
    }

    public bool OfferProduct(Product product, BakeState bakeState)
    {
        if (!_isInitialized || _customer == null)
        {
            Debug.LogWarning($"CustomerQuest {name}: Cannot offer product - quest not initialized");
            return false;
        }

        bool isBakedWell = bakeState != BakeState.Raw && bakeState != BakeState.FullBurn;
        bool fits = Check(product) && isBakedWell;

        if (fits)
        {
            if (_customer != null && _customer.DialogueManager != null && _playReactionSounds)
                _customer.DialogueManager.PlayAcceptSound();

            _productsLeft--;

            if (_productsLeft <= 0)
            {
                FinishQuest();
            }
            else
            {
                if (_customer.AnimatorController != null)
                    _customer.AnimatorController.OnItemAccepted();

                if (_interactions != null && _interactions.OnItemAccepted != null)
                {
                    _interactions.OnItemAccepted.PlayOut(_customer, DisplayQuestInteraction);
                }
            }
        }
        else
        {
            if (_customer != null && _customer.DialogueManager != null && _playReactionSounds)
                _customer.DialogueManager.PlayDenySound();

            if (_customer.AnimatorController != null)
                _customer.AnimatorController.OnItemRejected();

            if (_customDecline != null && _customDecline.IsValid())
            {
                _customDecline.PlayOut(_customer);
            }
            else if (_interactions != null && _interactions.OnItemRejected != null)
            {
                _interactions.OnItemRejected.PlayOut(_customer, DisplayQuestInteraction);
            }
        }

        return fits;
    }

    public void StartQuest()
    {
        Debug.Log($"[CustomerQuest] StartQuest called");

        if (!_isInitialized || _customer == null)
        {
            Debug.LogWarning($"CustomerQuest {name}: Cannot start quest - not properly initialized");
            return;
        }

        if (_customer.AnimatorController != null)
            _customer.AnimatorController.OnQuestStarted();

        if (_questInteraction != null)
        {
            Debug.Log("[CustomerQuest] Showing quest text");
            _customer.DialogueManager.TypingCompleted += OnQuestTextTypingCompleted;
            DisplayQuestInteraction();
        }
        else
        {
            Debug.LogWarning($"CustomerQuest {name}: QuestInteraction is missing");
            ShowDialogueOptions();
        }

        QuestStarted?.Invoke();
    }

    public void DisplayQuestInteraction()
    {
        if (_customer != null && _customer.DialogueManager != null)
            _customer.DialogueManager.SetCurrentCustomer(_customer);

        _customer.DialogueManager.DisplayText(_questInteraction.DialogueKey);
    }

    private void OnQuestTextTypingCompleted()
    {
        Debug.Log("[CustomerQuest] Quest text typing completed");

        if (_customer != null && _customer.DialogueManager != null)
            _customer.DialogueManager.TypingCompleted -= OnQuestTextTypingCompleted;

        ShowDialogueOptions();
    }

    private void ShowDialogueOptions()
    {
        if (_interactions != null && _customer.DialogueManager != null)
        {
            Debug.Log($"[CustomerQuest] Setting dialogue options. Count: {_interactions.DialogueOptions.Count}");
            _customer.DialogueManager.SetDialogueOptions(_interactions.DialogueOptions);
        }

        _customer.EnableReception();
    }

    private bool Check(Product product)
    {
        if (_applicableTypes == null || _applicableTypes.Count == 0 ||
            _applicableFillings == null || _applicableFillings.Count == 0)
            return true;

        bool typeFits = _applicableTypes.Any(type => product.Type == type || type == ProductType.Any);
        bool fillingFits = _applicableFillings.Any(filling => product.Filling == filling || filling == FillingType.Any);

        return typeFits && fillingFits;
    }

    private void FinishQuest()
    {
        if (_customer != null && _customer.DialogueManager != null)
            _customer.DialogueManager.TypingCompleted -= OnQuestTextTypingCompleted;

        _customer.DisableReception();
        _customer.DialogueManager.SetDialogueOptions(null);

        if (!_isInitialized || _customer == null)
        {
            Debug.LogWarning($"CustomerQuest {name}: Cannot finish quest - not properly initialized");
            return;
        }

        if (_customer.AnimatorController != null)
            _customer.AnimatorController.OnQuestFinished();

        if (_interactions != null && _interactions.OnQuestCompleted != null)
            _interactions.OnQuestCompleted.PlayOut(_customer, FinalizeQuest);
        else
            FinalizeQuest();
    }

    private void FinalizeQuest()
    {
        if (_customer != null)
        {
            _customer.DialogueManager.DisableBubble();
            _customer.FinishQuest();
        }

        _isInitialized = false;
        Debug.Log("Quest finished");
    }
}