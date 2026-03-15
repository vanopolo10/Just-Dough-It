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

[CreateAssetMenu(fileName = "CustomerQuest", menuName = "ScriptableObjects/CustomerSystem/CustomerQuest")]
public class CustomerQuest : ScriptableObject
{
    [SerializeField] private CustomerInteractionSet _interactions;
    [SerializeField] private CustomerInteraction _questInteraction;
    [SerializeField] private int _productsNeeded;
    [SerializeField] private List<ProductType> _applicableTypes;
    [SerializeField] private List<FillingType> _applicableFillings;

    private int _productsLeft;
    private Customer _customer;
    private bool _isInitialized;

    public CustomerInteractionSet Interactions => _interactions;
    public CustomerInteraction QuestInteraction => _questInteraction;

    public void Initialize(Customer customer)
    {
        Debug.Log($"[CustomerQuest] Initialize called for quest: {name}");
        
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
            Debug.Log("[CustomerQuest] Playing OnGreeting with callback to StartQuest");
            /*
            _customer.DialogueManager.DisplayTextWithCallback(
                _interactions.OnGreeting.DialogueKey, 
                StartQuest
            );
            */
            _interactions.OnGreeting.PlayOut(_customer, StartQuest);
        }
        else
        {
            Debug.LogWarning($"CustomerQuest {name}: OnGreeting is missing. Starting quest immediately.");
            StartQuest();
        }
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
            _customer.DialogueManager.DisplayText(_questInteraction.DialogueKey);
        }
        else
        {
            Debug.LogWarning($"CustomerQuest {name}: QuestInteraction is missing");
        }

        if (_interactions != null && _customer.DialogueManager != null)
        {
            Debug.Log($"[CustomerQuest] Setting dialogue options. Count: {_interactions.DialogueOptions.Count}");
            _customer.DialogueManager.SetDialogueOptions(_interactions.DialogueOptions);
        }
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
        if (!_isInitialized || _customer == null)
        {
            Debug.LogWarning($"CustomerQuest {name}: Cannot finish quest - not properly initialized");
            return;
        }
        
        if (_customer.AnimatorController != null)
            _customer.AnimatorController.OnQuestFinished();

        if (_interactions != null && _interactions.OnQuestCompleted != null)
        {
            // _customer.DialogueManager.DisplayText(_interactions.OnQuestCompleted.DialogueKey);
            _interactions.OnQuestCompleted.PlayOut(_customer, FinalizeQuest);
        }

        
    }

    public void FinalizeQuest() { //has to be public sadly
        if (_customer != null)
            _customer.FinishQuest();

        _isInitialized = false;
        Debug.Log("Quest finished");
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
}