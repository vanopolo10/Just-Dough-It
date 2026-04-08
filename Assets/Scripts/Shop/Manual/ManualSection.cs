using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerInteraction",
    menuName = "ScriptableObjects/Manual/ManualSection")]
public class ManualSection : ScriptableObject
{
    [SerializeField] private List<ManualPage> _pages = new();
    public IReadOnlyList<ManualPage> Pages => _pages;

    public void AddPage(ManualPage page)
    {
        _pages.Add(page);
    }
}
