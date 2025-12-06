using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public enum QuestInvokeType 
{ 
    SaleMade,
    PerfectAction
}
public class QuestSystem : MonoBehaviour
{
    [Serializable]
    struct QuestDisplay 
    {
        public string description;
        public QuestInvokeType type;
        public int maxScore;
        [SerializeField] private int _score;
        public int Score => _score;
        public QuestDisplay(QuestDisplay old, int score)
        { 
            description = old.description;
            type = old.type;
            maxScore = old.maxScore;
            _score = score;
        }
    }

    [SerializeField] private string completionText = " (Завершено!)";
    [SerializeField] private List<QuestDisplay> quests;
    public TextMeshProUGUI text;
    public UnityEvent OnFullCompletion;

    public static QuestSystem Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        string s = "";
        foreach (QuestDisplay quest in quests)
        { 
            s += quest.description;
            s += 
                quest.Score < quest.maxScore ?
                " (" + quest.Score + "/" + quest.maxScore + ")"
                :
                completionText;
            ;
            s += "" + '\n' + '\n';
        }

        text.text = s;
    }

    public void InvokeQuest(QuestInvokeType type)
    {
        int completed = 0;
        for (int i = 0; i < quests.Count; i++)  
        {
            QuestDisplay quest = quests[i];
            if (quest.type == type)
                quests[i] = new QuestDisplay(quest, quest.Score + 1);
            if(quest.Score >= quest.maxScore)
                completed++;
        }

        if(completed >= quests.Count)
            OnFullCompletion.Invoke();

        UpdateText();
    }
}
