using UnityEngine;

[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour
{
    [SerializeField] private WorldTime _worldTime;

    [Header("Rotation")]
    [SerializeField] private AnimationCurve _sunAngleCurve;
    [SerializeField] private float _yRotation = 25f;

    [Header("Light")]
    [SerializeField] private AnimationCurve _intensityCurve;
    [SerializeField] private Gradient _colorGradient;

    private bool _doPreferSunrises;
    private Light _light;

    private void Awake()
    {
        _light = GetComponent<Light>();
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
        float p = time.CompletePercent;

        if (p == 0)
            _doPreferSunrises = SaveSystem.LoadData<bool>(SaveSystem.SelectedSave, "DoPreferSunrises");
        
        float angle = _doPreferSunrises ? _sunAngleCurve.Evaluate(p) : _sunAngleCurve.Evaluate(180 - p);
        transform.rotation = Quaternion.Euler(angle, _yRotation, 0f);

        _light.intensity = _intensityCurve.Evaluate(p);
        _light.color = _colorGradient.Evaluate(p);
    }
}