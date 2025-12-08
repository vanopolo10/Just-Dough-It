using System;
using System.Collections.Generic;
using UnityEngine;
using static QuestSystem;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private MoneyManager _moneyManager;
    [SerializeField] private QuestSystem _questSystem;
    [SerializeField] private DoughBucket _doughBucket;
    [SerializeField] private Transform _shopTransfotm;

    private void Start()
    {
        int id = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "CameraViewID");
        _cameraController.SetViewID(id);

        int vibe = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "VibeLevel");
        Cafe.Instance.SetVibeLevel(vibe);

        int money = SaveSystem.LoadData<int>(SaveSystem.SelectedSave, "MoneyCount");
        _moneyManager.AddMoney(money);

        List<QuestDisplay> quests = SaveSystem.LoadData<List<QuestDisplay>>(SaveSystem.SelectedSave, "Quests");
        if (quests != null)
            _questSystem.SetQuests(quests);

        DoughSave dough = SaveSystem.LoadData<DoughSave>(SaveSystem.SelectedSave, "Dough");
        if (_doughBucket != null)
            _doughBucket.SpawnDough(dough.State, dough.Filling);

        BuyButtonContent[] buyedContent = _shopTransfotm.GetComponentsInChildren<BuyButtonContent>();
        foreach (BuyButtonContent content in buyedContent)
        {
            bool buyed = SaveSystem.LoadData<bool>(SaveSystem.SelectedSave, $"Buyable.{content.Key}");
            content.BuyableThing.SetActive(buyed);
            content.Back.SetActive(!buyed);
        }
    }

    public void Autosave()
    {
        Save("Autosave");
    }

    public void SaveGame(string saveName)
    {
        Save(saveName);
    }

    private void Save(string saveName)
    {
        SaveSystem.CreateSave(saveName);
        SaveSystem.SaveImage(saveName);
        SaveSystem.SaveData(saveName, "CameraViewID", _cameraController.ViewID);
        SaveSystem.SaveData(saveName, "VibeLevel", Cafe.Instance.VibeLevel);
        SaveSystem.SaveData(saveName, "MoneyCount", _moneyManager.Money);
        SaveSystem.SaveData(saveName, "Quests", _questSystem.Quests);
        if (_doughBucket.CurrentDough != null)
            SaveSystem.SaveData(saveName, "Dough", new DoughSave(_doughBucket.CurrentDough.State, _doughBucket.CurrentDough.Filling));

        BuyButtonContent[] buyedContent = _shopTransfotm.GetComponentsInChildren<BuyButtonContent>();
        foreach (BuyButtonContent content in buyedContent)
        {
            SaveSystem.SaveData(saveName, $"Buyable.{content.Key}", content.BuyableThing.activeSelf);
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
