using UnityEngine;

public class BakeVisual : MonoBehaviour
{
    [SerializeField] private int _materialIndex = 0;

    [SerializeField] private string _bakeProperty = "_bakeBlend";
    [SerializeField] private string _burnProperty = "_burnAmount";

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private BakeManager _bakeManager;
    
    private Material _instanceMaterial;

    private int _bakeId;
    private int _burnId;

    private void Awake()
    {
        _bakeId = Shader.PropertyToID(_bakeProperty);
        _burnId = Shader.PropertyToID(_burnProperty);

        var materials = _renderer.materials;
        if (_materialIndex < 0 || _materialIndex >= materials.Length)
        {
            enabled = false;
            return;
        }

        var source = materials[_materialIndex];
        if (source == null)
        {
            enabled = false;
            return;
        }

        _instanceMaterial = new Material(source);
        materials[_materialIndex] = _instanceMaterial;
        _renderer.materials = materials;

        float bakeT = _bakeManager != null ? Mathf.Clamp01(_bakeManager.CurrentBakeBlend) : 0f;
        float burnT = _bakeManager != null ? Mathf.Clamp01(_bakeManager.CurrentBurnAmount) : 0f;

        _instanceMaterial.SetFloat(_bakeId, bakeT);
        _instanceMaterial.SetFloat(_burnId, burnT);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying && _instanceMaterial != null)
            Destroy(_instanceMaterial);
    }

    private void Update()
    {
        float bakeT = Mathf.Clamp01(_bakeManager.CurrentBakeBlend);
        float burnT = Mathf.Clamp01(_bakeManager.CurrentBurnAmount);

        _instanceMaterial.SetFloat(_bakeId, bakeT);
        _instanceMaterial.SetFloat(_burnId, burnT);
    }
}
