using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SoundLevel : MonoBehaviour
{
    [SerializeField] private string _volumeParameter = "";
    [SerializeField] private AudioMixer _mixer;

    [Header("PrePlay")]
    [SerializeField] private List<AudioClip> _clips;
    [SerializeField] AudioMixerGroup _group;
    
    private Slider _slider;
    private AudioSource _audioSource;
    private bool _hasPrePlay;
    private float _lastPlay;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _hasPrePlay = _clips.Count > 0;
        if (_hasPrePlay)
        {
            _audioSource = gameObject.GetOrAddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = _group;
            _lastPlay = Time.time;
        }
        _slider.onValueChanged.AddListener(HandleSliderValueChange);
        _slider.value = PlayerPrefs.GetFloat(_volumeParameter, 0);
        _mixer.SetFloat(_volumeParameter, _slider.value);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(HandleSliderValueChange);
        PlayerPrefs.SetFloat(_volumeParameter, _slider.value);
    }

    private void HandleSliderValueChange(float value)
    {
        if (value < 0) value *= 2;
        if (value <= -39.9f) value = -80;
        _mixer.SetFloat(_volumeParameter, value);

        Preplay();
    }

    private void Preplay()
    {
        if (!_hasPrePlay || _lastPlay + 1f >= Time.time) return;
        _lastPlay = Time.time;

        _audioSource.PlayOneShot(_clips[Random.Range(0, _clips.Count)]);
    }
}
