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
    public Query query;
    public Product product;
    public UnityEvent OnAccept = null, OnDecline = null; // кодстайл пускай подождёт

    public void Offer()
    {
        bool result = query.Check(product);
        (result ? OnAccept : OnDecline).Invoke();
        //return result;
    }
}
