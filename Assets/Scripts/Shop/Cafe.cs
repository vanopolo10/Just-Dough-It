using System;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public class Cafe : MonoBehaviour
{
    public static Cafe Instance = null;
    
    [SerializeField] private Button _resetButton;
    
    public event Action DoughChanged;
    public event Action<DoughState> DoughStateChanged;
    
    public DoughController CurrentDough {get; private set;}
    public int VibeLevel { get; private set; } 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        _resetButton.onClick.AddListener(delegate {SetDoughState(DoughState.Raw);});
    }

    public void SetDoughState(DoughState doughState)
    {
        CurrentDough.SetState(doughState);
    }

    public void SetDough(DoughController doughController)
    {
        CurrentDough = doughController;
        CurrentDough.StateChanged += OnDoughStateChanged;
        DoughChanged?.Invoke();
    }

    private void OnDoughStateChanged()
    {
        DoughStateChanged?.Invoke(CurrentDough.State);
    }
}
