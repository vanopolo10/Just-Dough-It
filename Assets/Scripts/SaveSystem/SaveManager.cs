using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestSystem;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private MoneyManager _moneyManager;
    [SerializeField] private QuestSystem _questSystem;
    [SerializeField] private DoughBucket _doughBucket;
    [SerializeField] private Transform _shopTransform;
    [SerializeField] private GameObject _ui;
    [SerializeField] private DaysProgression _progression;
    [SerializeField] private WorldTime _time;

    private void Start()
    {
        string currentSave = SaveSystem.SelectedSave;

        if (_cameraController != null)
        {
            int id = SaveSystem.LoadData<int>(currentSave, "CameraViewID");
            _cameraController.SetViewID(id);
        }
        
        if (Cafe.Instance != null)
        {
            int vibe = SaveSystem.LoadData<int>(currentSave, "VibeLevel");
            Cafe.Instance.SetVibeLevel(vibe);
        }

        if (_moneyManager != null)
        {
            int money = SaveSystem.LoadData<int>(currentSave, "MoneyCount");
            _moneyManager.AddMoney(money, false);
        }
        
        if (_questSystem != null)
        {
            List<QuestDisplay> quests = SaveSystem.LoadData<List<QuestDisplay>>(currentSave, "Quests");
            if (quests != null)
                _questSystem.SetQuests(quests);
        }
        
        if (_doughBucket != null)
        {
            DoughSave dough = SaveSystem.LoadData<DoughSave>(currentSave, "Dough");
            if (_doughBucket != null)
                _doughBucket.SpawnDough(dough.State, dough.Filling);
        }
        
        if (_shopTransform != null)
        {
            BuyButtonContent[] boughtContent = _shopTransform.GetComponentsInChildren<BuyButtonContent>();
            foreach (BuyButtonContent content in boughtContent)
            {
                bool bought = SaveSystem.LoadData<bool>(currentSave, $"Buyable.{content.Key}");
                content.BuyableThing.SetActive(bought);
                content.Back.SetActive(!bought);
            }
        }
        
        if (_progression != null)
            _progression.SetDay(SaveSystem.LoadData<int>(currentSave, "Day"));

        if (_time != null)
            _time.SetGameTime(SaveSystem.LoadData<WorldTime.GameTime>(currentSave, "Time"));
    }

    public void SaveGame()
    {
        StartCoroutine(Save());
    }

    private IEnumerator Save()
    {
        string currentSave = SaveSystem.SelectedSave; 
        SaveSystem.CreateSave(currentSave);
        _ui.SetActive(false);
        yield return new WaitForEndOfFrame();
        SaveSystem.SaveImage(currentSave);
        
        if (_cameraController != null)
            SaveSystem.SaveData(currentSave, "CameraViewID", _cameraController.ViewID);
        
        if (Cafe.Instance != null)
            SaveSystem.SaveData(currentSave, "VibeLevel", Cafe.Instance.VibeLevel);
        
        if (_moneyManager != null)
            SaveSystem.SaveData(currentSave, "MoneyCount", _moneyManager.Money);
        
        if (_questSystem != null)
            SaveSystem.SaveData(currentSave, "Quests", _questSystem.Quests);
        
        if (_doughBucket != null && _doughBucket.CurrentDough != null)
            SaveSystem.SaveData(currentSave, "Dough", new DoughSave(_doughBucket.CurrentDough.State, _doughBucket.CurrentDough.Filling));

        if (_shopTransform != null)
        {
            BuyButtonContent[] boughtContent = _shopTransform.GetComponentsInChildren<BuyButtonContent>();
            foreach (BuyButtonContent content in boughtContent)
            {
                SaveSystem.SaveData(currentSave, $"Buyable.{content.Key}", content.BuyableThing.activeSelf);
                yield return null;
            }
        }

        if (_progression != null)
            SaveSystem.SaveData(currentSave, "Day", _progression.CurrentDay);

        if (_time != null)
            SaveSystem.SaveData(currentSave, "Time", _time.InGameTime);
    }

    [Serializable]
    private struct DoughSave
    {
        public DoughState State;
        public FillingType Filling;

        public DoughSave(DoughState state, FillingType filling)
        {
            State = state;
            Filling = filling;
        }
    }

    [Serializable]
    private struct BuyedStuff
    {
        public List<BuyButtonContent> Values;

        public BuyedStuff(List<BuyButtonContent> values)
        {
            Values = values;
        }
    }
}
