using System;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [Header("Money")] [SerializeField] private MoneyManager _moneyManager;
    [SerializeField] private int _basePrice = 30;
    [SerializeField] private int _qualityMultiplayer = 20;

    [SerializeField] private List<Transform> _places = new();
    [SerializeField] private GameObject _debugPie;
    [SerializeField] private bool _placeDebugPies;

    private readonly Queue<BakeManager> _queue = new();
    private BakeManager[] _occupied;

    private void Awake()
    {
        _occupied = new BakeManager[_places.Count];
    }

    private void Start()
    {
        if (!_placeDebugPies) return;

        for (int i = 0; i < 3; i++)
        {
            GameObject spawnedPie = Instantiate(_debugPie, transform);
            Place(spawnedPie.GetComponent<BakeManager>());
        }
    }

    public void Place(BakeManager bun)
    {
        if (bun == null)
            return;

        if (bun.BakeState is BakeState.Rare or BakeState.Burn)
            bun.ImperfectActionCount += 3;
        else if(bun.BakeState == BakeState.Done)
            bun.PerfectActionCount += 3;

        int index = FindFreeSlot();

        if (index >= 0)
        {
            PutToSlot(index, bun);
            bun.Sold += OnBunSold;
            bun.Threw += OnBunThrown;
        }
        else
        {
            _queue.Enqueue(bun);
            bun.gameObject.SetActive(false);
        }
    }

    private void OnBunThrown(BakeManager bun)
    {
        Remove(bun);
        Destroy(bun.gameObject);
    }
    
    private void OnBunSold(BakeManager bun)
    {
        _moneyManager.AddMoney(Mathf.FloorToInt(_basePrice + 1f * bun.PerfectActionCount / (bun.ImperfectActionCount + bun.PerfectActionCount) * _qualityMultiplayer));

        OnBunThrown(bun);
    }

    private void Remove(BakeManager bun)
    {
        if (bun == null)
            return;

        for (int i = 0; i < _occupied.Length; i++)
        {
            if (_occupied[i] != bun)
                continue;

            _occupied[i] = null;

            bun.Sold -= OnBunSold;

            TryFillSlot(i);
            break;
        }
    }

    private int FindFreeSlot()
    {
        for (int i = 0; i < _occupied.Length; i++)
        {
            if (_occupied[i] == null)
                return i;
        }

        return -1;
    }

    private void PutToSlot(int index, BakeManager bun)
    {
        _occupied[index] = bun;

        Transform anchor = _places[index];
        Transform t = bun.transform;

        t.gameObject.SetActive(true);

        t.SetParent(anchor, true);

        t.position = anchor.position;
        t.rotation = anchor.rotation;

        bun.OnPlacedOnShelf(anchor);
    }

    private void TryFillSlot(int index)
    {
        while (_queue.Count > 0)
        {
            BakeManager next = _queue.Dequeue();
            if (next == null)
                continue;

            PutToSlot(index, next);
            break;
        }
    }
}