using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    [SerializeField] private List<Transform> _points;

    public IReadOnlyList<Transform> Points => _points;

    public bool IsAvailable { get; private set; } = true;

    public void Occupy()
    {
        IsAvailable = false;
    }
    
    public void Free()
    {
        IsAvailable = true;
    }
}
