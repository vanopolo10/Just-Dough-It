using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoughBakeManager : MonoBehaviour
{
    [Header("Время прожарки, сек")]
    [SerializeField] private int _rareInSeconds = 3;
    [SerializeField] private int _doneInSeconds = 6;
    [SerializeField] private int _burnInSeconds = 9;

    [Header("Цвета прожарки")]
    [SerializeField] private Color _rareColor = Color.yellow;
    [SerializeField] private Color _doneColor = new(0.8f, 0.5f, 0.2f);
    [SerializeField] private Color _burnColor = Color.black;

    private MeshRenderer[] _meshRenderers;
    private Coroutine _bakeRoutine;
    private int _secondsInOven;

    public event Action Rare;
    public event Action Done;
    public event Action Burn;

    public BakeState BakeState { get; private set; } = BakeState.Raw;

    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();

        if (_meshRenderers == null || _meshRenderers.Length == 0)
            Debug.LogWarning("[DoughBakeManager] No MeshRenderers found on bun", this);
    }

    public void BeginBake()
    {
        print("Печка началась");
        
        if (_bakeRoutine != null)
            return;

        _bakeRoutine = StartCoroutine(BakeRoutine());
    }

    public void StopBake()
    {
        if (_bakeRoutine == null)
            return;

        StopCoroutine(_bakeRoutine);
        _bakeRoutine = null;
    }

    private IEnumerator BakeRoutine()
    {
        _secondsInOven = 0;
        BakeState = BakeState.Raw;

        while (true)
        {
            yield return new WaitForSeconds(1f);
            _secondsInOven++;
            CheckState();
        }
    }

    private void CheckState()
    {
        if (_secondsInOven == _rareInSeconds)
        {
            BakeState = BakeState.Rare;
            ApplyColor(_rareColor);
            Rare?.Invoke();
            Debug.Log("[DoughBakeManager] Rare", this);
        }
        else if (_secondsInOven == _doneInSeconds)
        {
            BakeState = BakeState.Done;
            ApplyColor(_doneColor);
            Done?.Invoke();
            Debug.Log("[DoughBakeManager] Done", this);
        }
        else if (_secondsInOven == _burnInSeconds)
        {
            BakeState = BakeState.Burn;
            ApplyColor(_burnColor);
            Burn?.Invoke();
            Debug.Log("[DoughBakeManager] Burn", this);
        }
    }

    private void ApplyColor(Color color)
    {
        if (_meshRenderers == null)
            return;

        foreach (MeshRenderer r in _meshRenderers)
        {
            if (!r)
                continue;
            
            Material[] mats = r.materials;
            
            foreach (var t in mats)
            {
                if (t)
                    t.color = color;
            }
        }
    }
}
