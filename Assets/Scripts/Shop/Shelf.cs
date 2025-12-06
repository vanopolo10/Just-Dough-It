using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
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

        int index = FindFreeSlot();

        if (index >= 0)
        {
            PutToSlot(index, bun);
        }
        else
        {
            _queue.Enqueue(bun);
            bun.gameObject.SetActive(false);
        }
    }

    public void Remove(BakeManager bun)
    {
        if (bun == null)
            return;

        for (int i = 0; i < _occupied.Length; i++)
        {
            if (_occupied[i] != bun)
                continue;

            _occupied[i] = null;

            bun.transform.SetParent(null, true);

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
