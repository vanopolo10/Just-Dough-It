using System.Collections.Generic;
using UnityEngine;

public class ManualSection : MonoBehaviour
{
    [SerializeField] private List<ManualPage> _pages = new();
    public IReadOnlyList<ManualPage> Pages => _pages;

    public void AddPage(ManualPage page)
    {
        _pages.Add(page);
    }
}
