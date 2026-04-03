using UnityEngine;
using System;
using System.Collections.Generic;

public class FrostManager : MonoBehaviour
{
    [SerializeField] private Texture2D _frostTexture;
    [SerializeField] private Texture2D _brushTexture;
    [SerializeField] private float _pixelBrushSize = 50f;
    [SerializeField] private float _brushSize = 0.1f;

    [SerializeField] private List<float> _baseOpacities;
    [SerializeField] private Color _frostColor;
    
    public event Action OnResetAll;
    public event Action<int> Warmed;

    public Texture2D FrostTexture => _frostTexture;
    public Texture2D BrushTexture => _brushTexture;
    public float PixelBrushSize => _pixelBrushSize;
    public float BrushSize => _brushSize;

    public IReadOnlyList<float> BaseOpacities => _baseOpacities;
    public Color FrostColor => _frostColor;
    
    public static FrostManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
        ResetAllWindows();
    }

    public void ResetAllWindows()
    {
        OnResetAll?.Invoke();
    }

    public void SetWarm(int warm)
    {
        Warmed?.Invoke(warm);
    }
}

