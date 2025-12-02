using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour
{
    [SerializeField] private int _traySpace = 3;
    [SerializeField] private List<Vector3> _localBakePositions;
    [SerializeField] private float _scaleMultiplier = 1f;

    private readonly List<DoughBakeManager> _doughs = new();

    public bool IsFull => _doughs.Count >= _traySpace;

    private void Start()
    {
        if (_traySpace != _localBakePositions.Count)
            Debug.LogWarning("Tray: positions count != tray space", this);
    }

    public DoughBakeManager AddDough(DoughBakeManager doughBakePrefab)
    {
        if (IsFull)
            return null;

        int index = _doughs.Count;

        Vector3 slotWorldPos = transform.TransformPoint(_localBakePositions[index]);

        DoughBakeManager instance = Instantiate(doughBakePrefab, transform);
        Transform t = instance.transform;

        Vector3 parentLossy = transform.lossyScale;
        Vector3 sourceWorldScale = doughBakePrefab.transform.lossyScale;
        Vector3 desiredWorldScale = sourceWorldScale * _scaleMultiplier;

        Vector3 localScale = new Vector3(
            parentLossy.x != 0f ? desiredWorldScale.x / parentLossy.x : desiredWorldScale.x,
            parentLossy.y != 0f ? desiredWorldScale.y / parentLossy.y : desiredWorldScale.y,
            parentLossy.z != 0f ? desiredWorldScale.z / parentLossy.z : desiredWorldScale.z
        );

        t.localScale = localScale;
        t.rotation = Quaternion.identity;
        t.position = slotWorldPos;

        Renderer renderer = instance.GetComponentInChildren<Renderer>();
        
        if (renderer != null)
        {
            float bottomY = renderer.bounds.min.y;
            float deltaY = slotWorldPos.y - bottomY;

            Vector3 pos = t.position;
            pos.y += deltaY;
            t.position = pos;
        }

        _doughs.Add(instance);
        return instance;
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
