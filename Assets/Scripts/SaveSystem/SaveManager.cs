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
    [SerializeField] private ShopManager _shopManager;

    private void Awake()
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
                _moneyManager.SetMoney(money);
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

        if (_shopManager != null)
        {

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
            if (SaveSystem.TryLoadData<ShelfStuff>(currentSave, "Shelf", out ShelfStuff shelfStuff))
            {
                for (int i = 0; i < _shelf.Occupied.Length; i++)
                {
                    if (shelfStuff.Buns.TryGetValue(i, out ShelfBun bun))
                    {
                        GameObject obj = Instantiate(_shelf.GetPrefabByType(bun.BakeData.Product.Type));
                        if (!obj.TryGetComponent<BakeManager>(out BakeManager bakeManager))
                            bakeManager = obj.GetComponentInChildren<BakeManager>();
                        bakeManager.SetData(bun.BakeData);
                        bakeManager.transform.localScale = bun.Scale.GetVector3();
                        if (!obj.TryGetComponent<BakeVisual>(out BakeVisual bakeVisual))
                            bakeVisual = obj.GetComponentInChildren<BakeVisual>();
                        bakeVisual.SetupAfterSave(bun.InitialScale.GetVector3());

                        FillingManager fillingManager = obj.GetComponentInChildren<FillingManager>();
                        if (fillingManager)
                            fillingManager.SetFillingWithoutController(bun.BakeData.Product.Filling);

                        _shelf.Place(bakeManager);
                    } 
                }
            }
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

        if (_shopManager != null)
        {
            PurshcasesStuff purshcasesStuff = new(false);
            purshcasesStuff.CurrentBook = _shopManager.CurrentBook;
            for (int i = 0; i < _shopManager.Books.Count; i++)
            {
                PurshcasesStuff.Book book;
                if (_shopManager.Books[i].GetType() == typeof(SingularShopBook))
                {
                    book = new(1);
                    book.IsBuyed[0] = ((SingularShopBook)_shopManager.Books[i]).IsBought;
                }
                else
                {
                    book = new(2);
                    for (int p = 0; p < _shopManager.Books.Count; p++)
                        book.IsBuyed[p] = ((MegaShopBook)_shopManager.Books[i]).Books[p].IsBought;
                }
                purshcasesStuff.Books.Add(i, book);
            }
            SaveSystem.SaveData(currentSave, "Books", purshcasesStuff);
        }

        if (_progression != null)
            SaveSystem.SaveData(currentSave, "Day", _progression.CurrentDay);

        if (_time != null)
            SaveSystem.SaveData(currentSave, "Time", _time.InGameTime);

        if (_shelf != null)
        {
            ShelfStuff shelfStuff = new(false);
            for (int i = 0; i < _shelf.Occupied.Length; i++)
            {
                BakeManager bakeManager = _shelf.Occupied[i];
                if (bakeManager != null)
                {
                    BakeVisual bakeVisual = bakeManager.gameObject.GetComponent<BakeVisual>();
                    Transform transform = bakeManager.transform;
                    ShelfBun bun = new(transform.localScale, bakeVisual.InitialScale, bakeManager.GetData());
                    shelfStuff.Buns.Add(i, bun);
                }
                SaveSystem.SaveData(currentSave, "Shelf", shelfStuff);
            }
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

    [Serializable]
    private struct ShelfStuff
    {
        public Dictionary<int, ShelfBun> Buns;

        public ShelfStuff(bool _)
        {
            Buns = new();
        }
    }

    [Serializable]
    public struct ShelfBun
    {
        public SerializableVector3 InitialScale;
        public SerializableVector3 Scale;
        public BakeManager.BakeManagerData BakeData;

        public ShelfBun(Vector3 initialScale, Vector3 scale, BakeManager.BakeManagerData bakeData)
        {
            InitialScale = new SerializableVector3(initialScale);
            Scale = new SerializableVector3(scale);
            BakeData = bakeData;
        }
    }

    [Serializable]
    public struct SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SerializableVector3(Vector3 vector)
        {
            X = vector.x;
            Y = vector.y;
            Z = vector.z;
        }

        public readonly Vector3 GetVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }

    [Serializable]
    public struct PurshcasesStuff
    {
        public int CurrentBook;
        public Dictionary<int, Book> Books;

        public PurshcasesStuff(bool _)
        {
            CurrentBook = 0;
            Books = new Dictionary<int, Book>();
        }

        public struct Book
        {
            public bool[] IsBuyed;

            public Book(int pages)
            {
                IsBuyed = new bool[pages];
            }
        }
    }
}