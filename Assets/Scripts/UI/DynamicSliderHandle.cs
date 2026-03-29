using UnityEngine;
using UnityEngine.UI;

public class DynamicSliderHandle : MonoBehaviour
{
    [SerializeField] private HandlePreset[] _handlePresets;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _handleImage;
    [SerializeField] private Image _glowImage;

    private float _sliderAmplitude;
    private float _nextValue;

    private void Start()
    {
        if(_slider != null)
        {
            _slider.onValueChanged.AddListener(GetSliderValue);
            _sliderAmplitude = _slider.maxValue - _slider.minValue;
        } 

        GetSliderValue(_slider.value);
    }

    private void GetSliderValue(float value)
    {
        HandlePreset handlePreset = null;

        foreach (var t in _handlePresets)
        {
            if (value >= t.Value)
                handlePreset = t;
            else
                break;
        }


        if (handlePreset != null && _handleImage != null)
            GetComponent<Image>().sprite = handlePreset.HandleSprite;

        if (_glowImage != null)
        {
            Color glowColor = _glowImage.color;
            glowColor.a = value/_sliderAmplitude; 
            _glowImage.color = glowColor;
        }
    }
 
}

[System.Serializable]
public class HandlePreset
{
    public Sprite HandleSprite;
    public float Value;
}
