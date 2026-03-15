using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;

public class DynamicSliderHandle : MonoBehaviour
{
    [SerializeField] private HandlePreset[] _handlePresets;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _handleImage;
    [SerializeField] private Image _glowImage;

    private float _sliderAmplitude;
    private float nextValue;

    void Start()
    {
        if(_slider != null)
        {
            _slider.onValueChanged.AddListener(GetSliderValue);
            _sliderAmplitude = _slider.maxValue - _slider.minValue;
        } 
        else
            Debug.LogWarning("Слайдер не указан");
        GetSliderValue(_slider.value);
    }

    void GetSliderValue(float value)
    {
        HandlePreset handlePreset = null;

        for (int i = 0; i < _handlePresets.Length; i++)
        {
            if (value >= _handlePresets[i].Value)
                handlePreset = _handlePresets[i];
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
