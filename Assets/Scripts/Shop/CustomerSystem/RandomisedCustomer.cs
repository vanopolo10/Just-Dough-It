using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomisedCustomer : Customer
{
    [Serializable]
    private struct WeightedQuest
    {
        public CustomerQuest Quest;
        public int Weight;
    }

    [SerializeField] private List<WeightedQuest> _quests;

    public CustomerQuest ChooseRandomQuest()
    {
        int totalWeight = 0;
        
        foreach (WeightedQuest wq in _quests)
            totalWeight += wq.Weight;

        int val = UnityEngine.Random.Range(0, totalWeight);

        foreach (WeightedQuest wq in _quests)
        {
            if (val < wq.Weight)
            {
                return wq.Quest;
            }

            val -= wq.Weight;
        }

        return _quests[0].Quest;
    }

    public new void Start()
    {
        _quest = ChooseRandomQuest();

        CustomerManager manager = FindAnyObjectByType<CustomerManager>();
        while(_quest == manager.LastQuest)
        {
            _quest = ChooseRandomQuest();
            Debug.Log("Customer Quest Rerolled!");
        }

        base.Start();
    }
}