using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField] private WorldTime _worldTime;

    [Header("Skybox Properties")]
    [SerializeField] private string _zenithProperty = "_ZenithColor";
    [SerializeField] private string _horizonProperty = "_HorizonColor";
    [SerializeField] private string _exposureProperty = "_Exposure";

    [Header("Gradients")]
    [SerializeField] private Gradient _zenithGradient;
    [SerializeField] private Gradient _horizonGradient;

    [Header("Exposure")]
    [SerializeField] private AnimationCurve _exposureCurve = AnimationCurve.Linear(0f, 0.25f, 1f, 1.2f);

    private Material _skyboxMaterial;

    private void Awake()
    {
        _skyboxMaterial = RenderSettings.skybox;
    }

    private void OnEnable()
    {
        if (_worldTime != null)
            _worldTime.TimeChanged += OnTimeChanged;
    }

    private void OnDisable()
    {
        if (_worldTime != null)
            _worldTime.TimeChanged -= OnTimeChanged;
    }

    private void Start()
    {
        if (_worldTime != null)
            OnTimeChanged(_worldTime.InGameTime);
    }

    private void OnTimeChanged(WorldTime.GameTime time)
    {
        if (!_skyboxMaterial)
            return;

        float p = Mathf.Clamp01(time.CompletePercent);

        if (_skyboxMaterial.HasProperty(_zenithProperty))
            _skyboxMaterial.SetColor(_zenithProperty, _zenithGradient.Evaluate(p));

        if (_skyboxMaterial.HasProperty(_horizonProperty))
            _skyboxMaterial.SetColor(_horizonProperty, _horizonGradient.Evaluate(p));

        if (_skyboxMaterial.HasProperty(_exposureProperty))
            _skyboxMaterial.SetFloat(_exposureProperty, _exposureCurve.Evaluate(p));

        DynamicGI.UpdateEnvironment();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && _worldTime != null && _skyboxMaterial != null)
            OnTimeChanged(_worldTime.InGameTime);
    }
#endif
}