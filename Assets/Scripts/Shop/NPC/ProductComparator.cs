using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct Product
{
    public ProductType type;
    public FillingType filling;
}
[Serializable]
public struct Query
{
    public ProductType type;
    public FillingType filling;
    public bool Check(Product product)
    {
        return (
        (product.type == type || type == ProductType.Any)
        &&
        (product.filling == filling || filling == FillingType.Any)
        );
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

        return result;
    }
}
