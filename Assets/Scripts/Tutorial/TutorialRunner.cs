using System.Collections.Generic;
using UnityEngine;

public class TutorialRunner : MonoBehaviour
{
    private Queue<ITutorialGate> _gates = new();

    private GameObject _currentIcon;

    public void StartTutorial(IEnumerable<ITutorialGate> gates)
    {
        foreach (var g in gates)
            _gates.Enqueue(g);

        RunNext();
    }

    private void RunNext()
    {
        if (_gates.Count == 0)
        {
            HideIcon();
            Debug.Log("Tutorial finished");
            return;
        }

        var gate = _gates.Dequeue();

        ShowIcon(gate.IconObject);

        gate.Completed += RunNext;
        gate.Enter();
    }

    private void ShowIcon(GameObject icon)
    {
        HideIcon();

        if (icon == null)
            return;

        _currentIcon = icon;
        _currentIcon.SetActive(true);
    }

    private void HideIcon()
    {
        if (_currentIcon == null) return;
        
        _currentIcon.SetActive(false);
        _currentIcon = null;
    }
}