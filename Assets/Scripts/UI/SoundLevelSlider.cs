using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SoundLevel : MonoBehaviour
{
    [SerializeField] private string _volumeParameter = "";
    [SerializeField] private AudioMixer _mixer;
    
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(HandleSliderValueChange);
        _slider.value = PlayerPrefs.GetFloat(_volumeParameter, 0);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(HandleSliderValueChange);
        PlayerPrefs.SetFloat(_volumeParameter, _slider.value);
    }

    private void HandleSliderValueChange(float value)
    {
        // Небольшие костыли для красивого но функционального слайдера
        if (value < 0) value *= 2;
        if (value <= -39.9f) value = -80;
        _mixer.SetFloat(_volumeParameter, value);
    }
}
