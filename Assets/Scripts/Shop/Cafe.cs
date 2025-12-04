using System;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public class Cafe : MonoBehaviour
{
    public static Cafe Instance = null;

    [SerializeField] private Button _resetButton;
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

    private void Start()
    {
        _resetButton.onClick.AddListener(delegate { SetDoughState(DoughState.Raw); });
    }

    public void SetDoughState(DoughState doughState)
    {
        if (CurrentDough == null)
            return;

        CurrentDough.SetState(doughState);
    }

    public void SetDough(DoughController doughController)
    {
        if (_doughBucket == null)
            return;

        _doughBucket.SetDough(doughController);
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

        DoughBakeManager bakeManagerPrefab = stateVisual.GetComponentInChildren<DoughBakeManager>(true);
        if (bakeManagerPrefab == null)
        {
            Debug.LogWarning("[Cafe] No DoughBakeManager on current state visual", stateVisual);
            return;
        }

        DoughBakeManager bakedInstance = _tray.AddDough(bakeManagerPrefab);
        if (bakedInstance == null)
        {
            Debug.Log("[Cafe] Tray is full, dough not sent");
            return;
        }

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
}
