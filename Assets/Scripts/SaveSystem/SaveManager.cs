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

    private void Start()
    {
        int id = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "CameraViewID");
        _cameraController.SetViewID(id);

        int vibe = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "VibeLevel");
        Cafe.Instance.SetVibeLevel(vibe);

        int money = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "MoneyCount");
        _moneyManager.AddMoney(money, false);

        List<QuestDisplay> quests = SaveSystem.LoadData<List<QuestDisplay>>(SaveSystem.SelectedSave, "Quests");
        if (quests != null)
            _questSystem.SetQuests(quests);

        DoughSave dough = SaveSystem.LoadData<DoughSave>(SaveSystem.SelectedSave, "Dough");
        if (_doughBucket != null)
            _doughBucket.SpawnDough(dough.State, dough.Filling);

        BuyButtonContent[] boughtContent = _shopTransform.GetComponentsInChildren<BuyButtonContent>();
        foreach (BuyButtonContent content in boughtContent)
        {
            bool bought = SaveSystem.LoadData<bool>(SaveSystem.SelectedSave, $"Buyable.{content.Key}");
            content.BuyableThing.SetActive(bought);
            content.Back.SetActive(!bought);
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
        _ui.SetActive(true);
        SaveSystem.SaveData(currentSave, "CameraViewID", _cameraController.ViewID);
        SaveSystem.SaveData(currentSave, "VibeLevel", Cafe.Instance.VibeLevel);
        SaveSystem.SaveData(currentSave, "MoneyCount", _moneyManager.Money);
        SaveSystem.SaveData(currentSave, "Quests", _questSystem.Quests);
        if (_doughBucket.CurrentDough != null)
            SaveSystem.SaveData(currentSave, "Dough", new DoughSave(_doughBucket.CurrentDough.State, _doughBucket.CurrentDough.Filling));

        BuyButtonContent[] boughtContent = _shopTransform.GetComponentsInChildren<BuyButtonContent>();
        foreach (BuyButtonContent content in boughtContent)
        {
            SaveSystem.SaveData(currentSave, $"Buyable.{content.Key}", content.BuyableThing.activeSelf);
            yield return null;
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
