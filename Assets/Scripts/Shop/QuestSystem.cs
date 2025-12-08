using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum QuestInvokeType
{
    SaleMade,
    PerfectAction
}

public class QuestSystem : MonoBehaviour
{
    [SerializeField] private DoughBucket _doughBucket;
    [SerializeField] private string _completionTextKey = "quest.completed";
    [SerializeField] private List<QuestDisplay> _quests;
    [SerializeField] private TMP_Text _text;
    
    private DoughController _doughController;

    public event Action Completed;
    public static QuestSystem Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        UpdateText();
        _doughBucket.CurrentDoughChanged += OnDoughChanged;
        OnDoughChanged();
        LocalizationManager.Instance.OnLanguageChange += UpdateText;
    }

    private void OnDisable()
    {
        _doughBucket.CurrentDoughChanged -= OnDoughChanged;

        if (_doughController != null)
            _doughController.ActionPerfected -= OnPerfectAction;
        
        LocalizationManager.Instance.OnLanguageChange -= UpdateText;
    }

    private void OnDoughChanged()
    {
        if (_doughController != null)
            _doughController.ActionPerfected -= OnPerfectAction;

        _doughController = _doughBucket.CurrentDough;

        if (_doughController != null)
            _doughController.ActionPerfected += OnPerfectAction;
    }

    private void OnPerfectAction()
    {
        InvokeQuest(QuestInvokeType.PerfectAction);
    }

    public void InvokeQuest(QuestInvokeType type)
    {
        _quests = _quests
            .Select(q => 
            {
                int newScore = q.Score + (q.Type == type ? 1 : 0);
                return new QuestDisplay(q, newScore);
            })
            .ToList();

        int completed = _quests.Count(q => q.Score >= q.MaxScore);

        if (completed >= _quests.Count && _quests.Count > 0)
            Completed?.Invoke();

        UpdateText();
    }

    private void UpdateText()
    {
        if (_text == null)
            return;

        string text = "";

        foreach (QuestDisplay quest in _quests)
        {
            text += LocalizationManager.Instance.SelectedTable.GetPair(quest.DescriptionKey);
            text += quest.Score < quest.MaxScore
                ? " (" + quest.Score + "/" + quest.MaxScore + ")"
                : " " + LocalizationManager.Instance.SelectedTable.GetPair(_completionTextKey);

            text += "\n\n";
        }

        _text.text = text;
    }

    [Serializable]
    struct QuestDisplay
    {
        public string DescriptionKey;
        public QuestInvokeType Type;
        public int MaxScore;
        public int Score;

        public QuestDisplay(QuestDisplay old, int score)
        {
            DescriptionKey = old.DescriptionKey;
            Type = old.Type;
            MaxScore = old.MaxScore;
            Score = score;
        }
    }
}
