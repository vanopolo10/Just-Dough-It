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
    [SerializeField] private Shelf _shelf;

    private void Start()
    {
        string currentSave = SaveSystem.SelectedSave;

        if (_cameraController != null)
        {
            if (SaveSystem.TryLoadData<int>(currentSave, "CameraViewID", out int id))
                _cameraController.SetViewID(id);
        }

        if (Cafe.Instance != null)
        {
            if (SaveSystem.TryLoadData<int>(currentSave, "VibeLevel", out int vibe))
                Cafe.Instance.SetVibeLevel(vibe);
        }

        if (_moneyManager != null)
        {
            if (SaveSystem.TryLoadData<int>(currentSave, "MoneyCount", out int money))
                _moneyManager.AddMoney(money, false);
        }

        if (_questSystem != null)
        {
            if (SaveSystem.TryLoadData<List<QuestDisplay>>(currentSave, "Quests", out List<QuestDisplay> quests) && quests != null)
                _questSystem.SetQuests(quests);
        }

        if (_doughBucket != null)
        {
            if (SaveSystem.TryLoadData<DoughSave>(currentSave, "Dough", out DoughSave dough))
                _doughBucket.SpawnDough(dough.State, dough.Filling);
        }

        if (_shopTransform != null)
        {
            BuyButtonContent[] boughtContent = _shopTransform.GetComponentsInChildren<BuyButtonContent>();
            foreach (BuyButtonContent content in boughtContent)
            {
                if (SaveSystem.TryLoadData<bool>(currentSave, $"Buyable.{content.Key}", out bool bought))
                {
                    content.BuyableThing.SetActive(bought);
                    content.Back.SetActive(!bought);
                }
            }
        }

        if (_progression != null)
        {
            if (SaveSystem.TryLoadData<int>(currentSave, "Day", out int day))
                _progression.SetDay(day);
        }

        if (_time != null)
        {
            if (SaveSystem.TryLoadData<WorldTime.GameTime>(currentSave, "Time", out WorldTime.GameTime time))
                _time.SetGameTime(time);
        }

        if (_shelf != null)
        {
            if (SaveSystem.TryLoadData<Shelf.ShelfSaveData>(currentSave, "ShelfData", out Shelf.ShelfSaveData shelfData) && shelfData != null)
                _shelf.LoadFromSaveData(shelfData);
        }
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

        if (_shelf != null)
        {
            Shelf.ShelfSaveData shelfData = _shelf.GetSaveData();
            SaveSystem.SaveData(currentSave, "ShelfData", shelfData);
        }
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