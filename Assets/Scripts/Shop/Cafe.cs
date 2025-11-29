using System;
using UnityEngine;
using UnityEngine.UI;

public class Cafe : MonoBehaviour
{
    public static Cafe Instance = null;
    
    [SerializeField] private Button _resetButton;
    
    public DoughController CurrentDough {get; set;}

    public event Action DoughChanged; 
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        _resetButton.onClick.AddListener(delegate {SetDoughState(DoughState.Raw);});
    }

    private void SetDoughState(DoughState doughState)
    {
        CurrentDough.SetState(doughState);
    }
    
    public void SetDough(DoughController doughState)
    {
        CurrentDough = doughState;
        DoughChanged?.Invoke();
    }
}
