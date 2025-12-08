using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

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

[Serializable]
public struct Query
{
    public ProductType Type;
    public FillingType Filling;
    
    public bool Check(Product product)
    {
        Debug.Log($"{product.Type}, {product.Filling}");
        Debug.Log($"{Type}, {Filling}");
        
        return (product.Type == Type || Type == ProductType.Any)
               && (product.Filling == Filling || Filling == FillingType.Any);
    }
}

public class ProductComparator : MonoBehaviour
{
    [SerializeField] private Query _query;
    [SerializeField] private Product _product;
    
    public UnityEvent OnAccept = null, OnDecline = null;

    public void SetProduct(Product product)
    {
        _product = product;
    }
    
    public void SetQuery(Query query)
    { 
        _query = query;
    }
    
    public bool OfferCurrentProduct()
    {
        bool result = _query.Check(_product);
        
        (result ? OnAccept : OnDecline).Invoke();

        if (result)
            QuestSystem.Instance.InvokeQuest(QuestInvokeType.SaleMade);

        return result;
    }
}
