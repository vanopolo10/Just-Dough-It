using UnityEngine;

[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour
{
    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private AnimationCurve _sunAngleCurve;
    [SerializeField] private float _yRotation = 25f;
    [SerializeField] private AnimationCurve _intensityCurve;
    [SerializeField] private Gradient _colorGradient;

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

    private float Remap(float t)
    {
        return _worldTime.PreferSunrise ? t : 1f - t;
    }

    private void OnTimeChanged(WorldTime.GameTime time)
    {
        float p = time.CompletePercent;
        _light.color = _colorGradient.Evaluate(p);
        
        float t = Remap(p);

        float angle = _sunAngleCurve.Evaluate(t);

        transform.rotation = Quaternion.Euler(angle, _yRotation, 0f);

        _light.intensity = _intensityCurve.Evaluate(t);
    }
}