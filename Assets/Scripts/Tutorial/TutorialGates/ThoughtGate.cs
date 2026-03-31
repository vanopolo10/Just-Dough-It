using System;
using UnityEngine;

public class ThoughtGate : ITutorialGate
{
    private PlayerThoughts _thoughts;
    private bool _doShowIcon;
    private string _key;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public ThoughtGate(PlayerThoughts thoughts, string key, bool doShowIcon = false, GameObject iconObject = null)
    {
        _thoughts = thoughts;
        _key = key;
        _doShowIcon = doShowIcon;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _thoughts.ThoughtCompleted += OnThoughtCompleted;
        _thoughts.Think(_key, _doShowIcon);
    }

    private void OnThoughtCompleted()
    {
        _thoughts.ThoughtCompleted -= OnThoughtCompleted;
        Completed?.Invoke();
    }
}