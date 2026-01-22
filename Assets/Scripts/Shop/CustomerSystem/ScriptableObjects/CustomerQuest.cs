using NUnit.Framework;
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
    public CustomerInteractionSet interactions;

    public int ProductsNeeded;
    public List<ProductType> ApplicableTypes;
    public List<FillingType> ApplicableFillings;

    public float timeoutOnCompletion = 3f;

    private int _productsLeft;
    private Customer _customer;

    public void Initialize(Customer customer)
    {
        _customer = customer;

        _productsLeft = ProductsNeeded;

        interactions.OnGreeting.PlayOut(customer);
    }
    private bool Check(Product product)
    {
        bool typeFits = false, fillingFits = false;

        foreach (ProductType type in ApplicableTypes) {
            if (product.Type == type || type == ProductType.Any) { 
                typeFits = true;
                break;
            }
        }

        foreach (FillingType filling in ApplicableFillings)
        {
            if (product.Filling == filling || filling == FillingType.Any)
            {
                fillingFits = true;
                break;
            }
        }

        return (typeFits && fillingFits);
    }

    public void FinishQuest() {
        interactions.OnQuestCompleted.PlayOut(_customer);

        _customer.FinishQuest();

        _customer.DialogueManager.Timeout(timeoutOnCompletion);
    }

    public bool OfferProduct(Product product) { 
        bool fits = Check(product);

        if (fits)
        {
            _productsLeft--;
            if (_productsLeft <= 0)
                FinishQuest();
            else
                interactions.OnItemAccepted.PlayOut(_customer);
        }
        else
        {
            interactions.OnItemRejected.PlayOut(_customer);
        }

        return fits;
    }
}
