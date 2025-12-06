using System;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-1000)]
public class Cafe : MonoBehaviour
{
    public static Cafe Instance = null;

    [SerializeField] private OvenSender _ovenSender;
    [SerializeField] private Tray _tray;
    [SerializeField] private DoughBucket _doughBucket;

    public event Action DoughChanged;
    public event Action<DoughState> DoughStateChanged;

    public DoughController CurrentDough => _doughBucket != null ? _doughBucket.CurrentDough : null;
    public int VibeLevel { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        if (_ovenSender != null)
            _ovenSender.DoughSent += OnDoughSent;

        if (_doughBucket != null)
        {
            _doughBucket.CurrentDoughChanged += OnBucketDoughChanged;
            _doughBucket.DoughStateChanged += OnBucketDoughStateChanged;
        }
    }

    private void OnDisable()
    {
        if (_ovenSender != null)
            _ovenSender.DoughSent -= OnDoughSent;

        if (_doughBucket != null)
        {
            _doughBucket.CurrentDoughChanged -= OnBucketDoughChanged;
            _doughBucket.DoughStateChanged -= OnBucketDoughStateChanged;
        }
    }

    private void OnDoughSent()
    {
        DoughController dough = CurrentDough;
        if (dough == null)
            return;

        DoughVisualSwitcher visualSwitcher = dough.GetComponent<DoughVisualSwitcher>();
        if (visualSwitcher == null)
        {
            Debug.LogWarning("[Cafe] CurrentDough has no DoughVisualSwitcher", dough);
            return;
        }

        if (visualSwitcher.Map.TryGetValue(dough.State, out GameObject stateVisual) == false || stateVisual == null)
        {
            Debug.LogWarning($"[Cafe] No visual for state {dough.State}", dough);
            return;
        }

        BakeManager bakeManagerPrefab = stateVisual.GetComponentInChildren<BakeManager>(true);
        if (bakeManagerPrefab == null)
        {
            Debug.LogWarning("[Cafe] No DoughBakeManager on current state visual", stateVisual);
            return;
        }

        BakeManager bakedInstance = _tray.AddDough(bakeManagerPrefab);
        if (bakedInstance == null)
        {
            Debug.Log("[Cafe] Tray is full, dough not sent");
            return;
        }

        bakedInstance.SetPerfectActionCount(dough.PerfectActionCount);
        bakedInstance.SetImperfectActionCount(dough.ImperfectActionCount);
        bakedInstance.SetDoughInfo(dough.State, dough.Filling);

        if (_doughBucket != null)
            _doughBucket.SetDough(null);

        Destroy(dough.gameObject);
    }

    private void OnBucketDoughChanged()
    {
        DoughChanged?.Invoke();
    }

    private void OnBucketDoughStateChanged(DoughState state)
    {
        DoughStateChanged?.Invoke(state);
    }

    public void SetVibeLevel(int level)
    {
        VibeLevel = Math.Clamp(level, 0, 100_000);
    }
}
