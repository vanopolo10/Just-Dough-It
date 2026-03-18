using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Slider), typeof(AudioSource))]
public class SoundLevelSlider : MonoBehaviour
{
    private const float PlayKD = 1f;

    [SerializeField] private string _volumeParameter = "";
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private bool _hasPreview;
    [SerializeField] private string _groupName = "";
    [SerializeField] private AudioClip[] _clips;
    
    private Slider _slider;
    private AudioSource _source;
    private float _lastPlayTime;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        if (_hasPreview)
        {
            _source = GetComponent<AudioSource>();
            _source.outputAudioMixerGroup = _mixer.FindMatchingGroups(_groupName)[0];
            _source.playOnAwake = false;
            _lastPlayTime = Time.time;
        }
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
        if (_hasPreview && _lastPlayTime + PlayKD < Time.time)
        {
            _lastPlayTime = Time.time;
            _source.clip = RandomClip();
            _source.Play();
        }
    }

    private AudioClip RandomClip()
    {
        if (_clips.Length == 0) return null;
        return _clips[Random.Range(0, _clips.Length - 1)];
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SoundLevelSlider))]
public class SoundLevelSliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_volumeParameter"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_mixer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_hasPreview"));

        if (serializedObject.FindProperty("_hasPreview").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_groupName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_clips"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
