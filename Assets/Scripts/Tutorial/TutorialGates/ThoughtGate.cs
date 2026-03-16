using System;
using UnityEngine;

public class ThoughtGate : ITutorialGate
{
    private PlayerThoughts _thoughts;
    private string _key;

    public event Action Completed;
    public GameObject IconObject { get; }
    
    public ThoughtGate(PlayerThoughts thoughts, string key, GameObject iconObject = null)
    {
        _thoughts = thoughts;
        _key = key;
        IconObject = iconObject;
    }

    public void Enter()
    {
        _thoughts.ThoughtCompleted += OnThoughtCompleted;
        _thoughts.Think(_key);
    }

    private void OnThoughtCompleted()
    {
        _thoughts.ThoughtCompleted -= OnThoughtCompleted;
        Completed?.Invoke();
    }
}