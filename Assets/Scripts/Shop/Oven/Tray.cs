using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Tray : MonoBehaviour
{
    [Header("Печь")]
    [SerializeField] private Oven _oven;

    [Header("Движение подноса")]
    [SerializeField] private Vector3 _outsidePoint;
    [SerializeField] private Vector3 _insidePoint;
    [SerializeField] private float _moveDuration = 0.75f;
    [SerializeField] private AnimationCurve _moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Слоты для булочек")]
    [SerializeField] private List<TraySlot> _slots = new();

    [Header("Полка для булочек")]
    [SerializeField] private Shelf _shelf;

    private bool _isInOven;
    private bool _isMoving;
    private Coroutine _moveRoutine;

    private float _bakeSpeedMultiplier;

    public bool IsInOven => _isInOven;
    public bool IsMoving => _isMoving;
    public bool IsFull => _slots.All(t => !t.IsEmpty);

    public float BakeSpeedMultiplier => _bakeSpeedMultiplier;

    private void OnEnable()
    {
        if (_oven != null)
            _oven.FirePowerChanged += OnFirePowerChanged;
    }

    private void OnDisable()
    {
        if (_oven != null)
            _oven.FirePowerChanged -= OnFirePowerChanged;
    }

    private void OnFirePowerChanged(int firePower)
    {
        _bakeSpeedMultiplier = Mathf.Clamp(firePower / 50f, 0f, 2f);
    }

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    private void OnMouseDown()
    {
        if (_isMoving)
            return;

        bool toOven = !_isInOven;

        TogglePosition();

        foreach (var bun in _slots.Select(s => s.Bun).Where(b => b != null))
        {
            if (toOven)
                bun.BeginBake();
            else
                bun.StopBake();
        }
    }

    private void TogglePosition()
    {
        if (_isInOven)
            MoveTo(_outsidePoint, false);
        else
            MoveTo(_insidePoint, true);
    }

    public BakeManager AddDough(BakeManager prefab)
    {
        if (prefab == null)
            return null;

        TraySlot freeSlot = _slots.FirstOrDefault(t => t.IsEmpty);
        if (freeSlot == null)
            return null;

        BakeManager instance = Instantiate(
            prefab,
            freeSlot.Anchor.position,
            freeSlot.Anchor.rotation,
            freeSlot.Anchor
        );

        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.LookRotation(transform.right);

        freeSlot.SetBun(instance);
        instance.Setup(this, _shelf);

        if (_isInOven)
            instance.BeginBake();

        return instance;
    }

    public IEnumerable<BakeManager> TakeAll()
    {
        return from t in _slots where t.Bun != null select t.Clear();
    }

    public bool TryTakeBun(BakeManager bun, out BakeManager taken)
    {
        taken = null;

        foreach (var slot in _slots.Where(t => t.Bun == bun))
        {
            taken = slot.Clear();
            
            if (taken != null)
                taken.transform.SetParent(null, true);

            return true;
        }

        return false;
    }

    private void MoveTo(Vector3 targetPosition, bool toOven)
    {
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(MoveRoutine(targetPosition, toOven));
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition, bool toOven)
    {
        _isMoving = true;

        Vector3 start = transform.position;
        float time = 0f;

        while (time < _moveDuration)
        {
            float t = _moveCurve.Evaluate(time / _moveDuration);
            transform.position = Vector3.Lerp(start, targetPosition, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        _isInOven = toOven;
        _isMoving = false;
        _moveRoutine = null;
    }

    [System.Serializable]
    private class TraySlot
    {
        [SerializeField] private Transform _anchor;
        private BakeManager _bun;

        public Transform Anchor => _anchor;
        public BakeManager Bun => _bun;
        public bool IsEmpty => _bun == null;

        public void SetBun(BakeManager bun) => _bun = bun;

        public BakeManager Clear()
        {
            BakeManager result = _bun;
            _bun = null;
            return result;
        }
    }
}
