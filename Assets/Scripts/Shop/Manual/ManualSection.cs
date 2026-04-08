using System.Collections.Generic;
using UnityEngine;

public class ManualSection : MonoBehaviour
{
    private List<ManualPage> _pages = new();

    public void AddPage(ManualPage page)
    {
        _pages.Add(page);
    }
}
