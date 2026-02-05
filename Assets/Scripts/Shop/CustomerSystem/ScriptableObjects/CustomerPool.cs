using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct WeigtedObject { 
    public GameObject Customer;
    public int Weight;
};

[CreateAssetMenu(fileName = "CustomerPool", menuName = "ScriptableObjects/CustomerSystem/CustomerPool")]
public class CustomerPool : ScriptableObject
{
    [SerializeField] private List<WeigtedObject> _pool;

    public GameObject GetCustomerfromPool() {
        int totalWeight = 0;
        foreach (WeigtedObject customer in _pool) totalWeight += customer.Weight;

        int val = UnityEngine.Random.Range(0, totalWeight);

        foreach (WeigtedObject wc in _pool)
        {
            if (val < wc.Weight)
            {
                return wc.Customer;
            }
            val -= wc.Weight;
        }

        return _pool[0].Customer;
    }
}
