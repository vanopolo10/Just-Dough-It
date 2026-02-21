using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerPool", menuName = "ScriptableObjects/CustomerSystem/CustomerPool")]
public class CustomerPool : ScriptableObject
{
    [SerializeField] private List<WeightedObject> _pool;

    public int Count => _pool.Count;

    public GameObject GetCustomerFromPool()
    {
        int totalWeight = 0;
        foreach (WeightedObject customer in _pool) totalWeight += customer.Weight;

        int val = UnityEngine.Random.Range(0, totalWeight);

        foreach (WeightedObject wc in _pool)
        {
            if (val < wc.Weight)
                return wc.Customer;

            val -= wc.Weight;
        }

        return _pool[0].Customer;
    }
}

[Serializable]
public struct WeightedObject
{
    public GameObject Customer;
    public int Weight;
}