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
    
    public event Action DoughChanged;
    public event Action<DoughState> DoughStateChanged;
    
    public DoughController CurrentDough { get; private set; }
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
    }

    private void OnDisable()
    {
        if (_ovenSender != null)
            _ovenSender.DoughSent -= OnDoughSent;
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
        DoughStateChanged?.Invoke(doughState);
    }

    public void SetDough(DoughController doughController)
    {
        if (CurrentDough != null)
            CurrentDough.StateChanged -= OnDoughStateChanged;
        
        CurrentDough = doughController;
        
        if (CurrentDough != null)
            CurrentDough.StateChanged += OnDoughStateChanged;
        
        DoughChanged?.Invoke();
    }

    private void OnDoughSent()
    {
        if (CurrentDough == null)
            return;

        var visualSwitcher = CurrentDough.GetComponent<DoughVisualSwitcher>();
        if (visualSwitcher == null)
        {
            Debug.LogWarning("[Cafe] CurrentDough has no DoughVisualSwitcher", CurrentDough);
            return;
        }

        if (visualSwitcher.Map.TryGetValue(CurrentDough.State, out var stateVisual) == false || stateVisual == null)
        {
            Debug.LogWarning($"[Cafe] No visual for state {CurrentDough.State}", CurrentDough);
            return;
        }

        var bakeManagerPrefab = stateVisual.GetComponentInChildren<DoughBakeManager>(true);
        if (bakeManagerPrefab == null)
        {
            Debug.LogWarning("[Cafe] No DoughBakeManager on current state visual", stateVisual);
            return;
        }

        var bakedInstance = _tray.AddDough(bakeManagerPrefab);
        
        if (bakedInstance == null)
        {
            Debug.Log("[Cafe] Tray is full, dough not sent");
            return;
        }

        CurrentDough.StateChanged -= OnDoughStateChanged;
        Destroy(CurrentDough.gameObject);
        CurrentDough = null;

        DoughChanged?.Invoke();
    }

    
    private void OnDoughStateChanged()
    {
        if (CurrentDough == null)
            return;
        
        DoughStateChanged?.Invoke(CurrentDough.State);
    }
}
