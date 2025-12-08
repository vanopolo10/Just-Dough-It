using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [Header("Money")]
    [SerializeField] private MoneyManager _moneyManager;
    [SerializeField] private int _basePrice = 30;
    [SerializeField] private int _qualityMultiplayer = 20;
    
    [SerializeField] private List<Transform> _places = new();
    
    private readonly Queue<BakeManager> _queue = new();
    private BakeManager[] _occupied;

    private void Awake()
    {
        _occupied = new BakeManager[_places.Count];
    }

    public void Place(BakeManager bun)
    {
        if (bun == null)
            return;

        if (bun.BakeState != BakeState.Done)
            bun.ImperfectActionCount++;
        else
            bun.PerfectActionCount++;

        int index = FindFreeSlot();

        if (index >= 0)
        {
            PutToSlot(index, bun);
            bun.Sold += OnBunSold;
        }
        else
        {
            _queue.Enqueue(bun);
            bun.gameObject.SetActive(false);
        }
    }
    
    private void OnBunSold(BakeManager bun)
    {
        Remove(bun);
        _moneyManager.AddMoney(_basePrice + bun.PerfectActionCount / (bun.ImperfectActionCount + bun.PerfectActionCount) * _qualityMultiplayer);
        Destroy(bun.gameObject);
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
