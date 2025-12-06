using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Tray : MonoBehaviour
{
    [System.Serializable]
    private class TraySlot
    {
        [SerializeField] private Transform _anchor;
        private DoughBakeManager _bun;

        public Transform Anchor => _anchor;
        public DoughBakeManager Bun => _bun;
        public bool IsEmpty => _bun == null;

        public void SetBun(DoughBakeManager bun)
        {
            _bun = bun;
        }

        public DoughBakeManager Clear()
        {
            DoughBakeManager result = _bun;
            _bun = null;
            return result;
        }
    }

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

    public bool IsInOven => _isInOven;
    public bool IsMoving => _isMoving;

    public bool IsFull => _slots.All(t => !t.IsEmpty);

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

        _slots
            .Select(s => s.Bun)
            .Where(b => b != null)
            .ToList()
            .ForEach(b =>
            {
                if (toOven)
                    b.BeginBake();
                else
                    b.StopBake();
            });
    }

    private void TogglePosition()
    {
        if (_isInOven)
            MoveTo(_outsidePoint, false);
        else
            MoveTo(_insidePoint, true);
    }

    public DoughBakeManager AddDough(DoughBakeManager prefab)
    {
        if (prefab == null)
            return null;

        TraySlot freeSlot = _slots.FirstOrDefault(t => t.IsEmpty);

        if (freeSlot == null)
            return null;

        DoughBakeManager instance = Instantiate(
            prefab,
            freeSlot.Anchor.position,
            freeSlot.Anchor.rotation,
            freeSlot.Anchor
        );

        Vector3 parentScale = freeSlot.Anchor.lossyScale;
        Vector3 worldScale = prefab.transform.lossyScale;

        Vector3 localScale = new Vector3(
            parentScale.x == 0f ? 1f : worldScale.x * parentScale.x,
            parentScale.y == 0f ? 1f : worldScale.y * parentScale.y,
            parentScale.z == 0f ? 1f : worldScale.z * parentScale.z
        );

        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.LookRotation(transform.right);
        instance.transform.localScale = localScale;

        freeSlot.SetBun(instance);

        instance.Setup(this, _shelf);

        if (_isInOven)
            instance.BeginBake();

        return instance;
    }

    public IEnumerable<DoughBakeManager> TakeAll()
    {
        return from t in _slots where t.Bun != null select t.Clear();
    }

    public bool TryTakeBun(DoughBakeManager bun, out DoughBakeManager taken)
    {
        taken = null;

        if (bun == null)
            return false;

        foreach (var t in _slots.Where(t => t.Bun == bun))
        {
            taken = t.Clear();
            if (taken != null)
                taken.transform.SetParent(null, true);

            return taken != null;
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
            float t = time / _moveDuration;
            t = _moveCurve.Evaluate(t);

            transform.position = Vector3.Lerp(start, targetPosition, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        _isInOven = toOven;
        _isMoving = false;
        _moveRoutine = null;
    }
}
