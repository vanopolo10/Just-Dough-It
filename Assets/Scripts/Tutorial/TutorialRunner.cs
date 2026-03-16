using System.Collections.Generic;
using UnityEngine;

public class TutorialRunner : MonoBehaviour
{
    private Queue<ITutorialGate> _gates = new();

    private ITutorialGate _currentGate;
    private GameObject _currentIcon;

    public void StartTutorial(IEnumerable<ITutorialGate> gates)
    {
        foreach (var g in gates)
            _gates.Enqueue(g);

        RunNext();
    }

    private void RunNext()
    {
        if(_currentGate != null)
            _currentGate.Completed -= RunNext;
        
        if (_gates.Count == 0)
        {
            Debug.Log("Tutorial finished");
            return;
        }

        _currentGate = _gates.Dequeue();

        ShowIcon(_currentGate.IconObject);

        _currentGate.Completed += RunNext;
        _currentGate.Enter();
    }

    private void ShowIcon(GameObject icon)
    {
        HideIcon();

        if (!icon)
            return;

        _currentIcon = icon;
        _currentIcon.SetActive(true);
    }

    private void HideIcon()
    {
        if (!_currentIcon) return;
        
        _currentIcon.SetActive(false);
        _currentIcon = null;
    }
}