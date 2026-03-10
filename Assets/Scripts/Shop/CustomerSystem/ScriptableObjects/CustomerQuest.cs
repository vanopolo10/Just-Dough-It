using System;
using System.Collections.Generic;
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

[CreateAssetMenu(fileName = "CustomerQuest", menuName = "ScriptableObjects/CustomerSystem/CustomerQuest")]
public class CustomerQuest : ScriptableObject
{
    [SerializeField] private CustomerInteractionSet _interactions;
    [SerializeField] private CustomerInteraction _questInteraction;
    [SerializeField] private int _productsNeeded;
    [SerializeField] private List<ProductType> _applicableTypes;
    [SerializeField] private List<FillingType> _applicableFillings;
    [SerializeField] private float _timeoutOnCompletion = 3f;
    [SerializeField] private float _timeoutAfterGreeting = 3f;

    private int _productsLeft;
    private Customer _customer;
    private bool _isInitialized;
    private bool _isWaitingForText;

    public CustomerInteractionSet Interactions => _interactions;
    public CustomerInteraction QuestInteraction => _questInteraction;

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
            _interactions.OnGreeting.PlayOut(_customer, null, 0f);

            if (_customer.DialogueManager != null && !_isWaitingForText)
            {
                _customer.DialogueManager.TextDisplayed += OnGreetingCompleted;
                _isWaitingForText = true;
            }
        }
        else
        {
            Debug.LogWarning($"CustomerQuest {name}: OnGreeting interaction is missing");
            _customer.Invoke(nameof(_customer.StartQuest), _timeoutAfterGreeting);
        }
        
        Debug.Log($"Customer Initialized");
    }

    private void OnGreetingCompleted()
    {
        if (!_customer || !_customer.DialogueManager)
        {
            Debug.LogError($"CustomerQuest {name}: Customer or DialogueManager is null in OnGreetingCompleted");
            Cleanup();
            return;
        }

        _customer.DialogueManager.TextDisplayed -= OnGreetingCompleted;
        _isWaitingForText = false;

        _customer.Invoke(nameof(_customer.StartQuest), _timeoutAfterGreeting);
    }

    public void StartQuest()
    {
        if (!_isInitialized || _customer == null)
        {
            Debug.LogWarning($"CustomerQuest {name}: Cannot start quest - not properly initialized");
            return;
        }
        
        if (_customer.AnimatorController != null)
            _customer.AnimatorController.OnQuestStarted();

        if (_questInteraction != null)
        {
            _questInteraction.PlayOut(_customer);
        }
        else
        {
            Debug.LogWarning($"CustomerQuest {name}: QuestInteraction is missing");
        }

        if (_interactions != null && _customer.DialogueManager != null)
            _customer.DialogueManager.SetDialogueOptions(_interactions.DialogueOptions);
    }

    private bool Check(Product product)
    {
        if (_applicableTypes == null || _applicableTypes.Count == 0 ||
            _applicableFillings == null || _applicableFillings.Count == 0)
            return true;

        bool typeFits = false;
        bool fillingFits = false;

        foreach (ProductType type in _applicableTypes)
        {
            if (product.Type == type || type == ProductType.Any)
            {
                typeFits = true;
                break;
            }
        }

        foreach (FillingType filling in _applicableFillings)
        {
            if (product.Filling == filling || filling == FillingType.Any)
            {
                fillingFits = true;
                break;
            }
        }

        return typeFits && fillingFits;
    }

    public void FinishQuest()
    {
        if (!_isInitialized || _customer == null)
        {
            Debug.LogWarning($"CustomerQuest {name}: Cannot finish quest - not properly initialized");
            return;
        }
        
        if (_customer.AnimatorController != null)
            _customer.AnimatorController.OnQuestFinished();

        if (_interactions != null && _interactions.OnQuestCompleted != null)
        {
            _interactions.OnQuestCompleted.PlayOut(_customer);
        }

        if (_customer != null)
            _customer.FinishQuest();

        if (_customer != null && _customer.DialogueManager != null)
            _customer.DialogueManager.Timeout(_timeoutOnCompletion);

        _isInitialized = false;
        Debug.Log($"Quest finished");
    }

    public bool OfferProduct(Product product)
    {
        if (!_isInitialized || _customer == null)
        {
            Debug.LogWarning($"{_customer} CustomerQuest {name}: Cannot offer product - quest not initialized");
            return false;
        }

        bool fits = Check(product);

        if (fits)
        {
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
                    _interactions.OnItemAccepted.PlayOut(_customer);
            }
        }
        else
        {
            if (_customer.AnimatorController != null)
                _customer.AnimatorController.OnItemRejected();

            if (_interactions != null && _interactions.OnItemRejected != null)
                _interactions.OnItemRejected.PlayOut(_customer);
        }

        return fits;
    }

    public void ResetQuest()
    {
        if (_customer != null && _customer.DialogueManager != null && _isWaitingForText)
        {
            _customer.DialogueManager.TextDisplayed -= OnGreetingCompleted;
            _isWaitingForText = false;
        }
        
        _productsLeft = _productsNeeded;
        _isInitialized = false;
        _customer = null;
        Debug.Log($"Quest reset");
    }

    private void Cleanup()
    {
        _isWaitingForText = false;
    }
}