using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MultiQuestCustomer : Customer
{
    [SerializeField] private List<CustomerQuest> _quests;
    private int _questId = 0;
    protected override void Start() { 
        base.Start();
        _questId = 0;
        _quest = _quests[0];
    }
    public override void FinishQuest() {
        Debug.Log($"[MultiQuestCustomer] Finished quest with index {_questId}.");
        _questId++;
        if (_questId >= _quests.Count)
        {
            Debug.Log($"[MultiQuestCustomer] No more quests");
            FinishAllQuests();
        }
        else
        {
            _quest = _quests[_questId];
            Initialize();
            Debug.Log($"[MultiQuestCustomer] Started quest with index {_questId}.");
        }
    }
    private void FinishAllQuests() {
        Debug.Log($"[MultiQuestCustomer] Finished all quests.");
        base.FinishQuest();
    }
}
