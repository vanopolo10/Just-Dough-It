using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    private List<DoughBakeManager> _doughs;

    public void AddDough(DoughBakeManager doughBakeManager)
    {
        _doughs.Add(doughBakeManager);
    }

    private void BeginBake()
    {
        foreach (var dough in _doughs)
            dough.BeginBake();
    }
    
    private void StopBake()
    {
        foreach (var dough in _doughs)
            dough.StopBake();
    }
}
