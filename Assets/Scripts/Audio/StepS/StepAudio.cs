using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class StepAudio : MonoBehaviour
{
    [SerializeField] private StepConfig _config;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (_config == null)
        {
            enabled = false;
            return;
        }
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.volume = 0.4f;
        _audioSource.spatialBlend = 1f;
        _audioSource.playOnAwake = false;
        _audioSource.outputAudioMixerGroup = _config.Group;
    }

    public void Step()
    {
        Ray ray = new(transform.position + transform.up * 0.01f, -transform.up + transform.forward * 0.5f);
        RaycastHit[] hits = Physics.RaycastAll(ray, 10f);
        foreach (RaycastHit hit in hits)
            if (hit.collider.gameObject.TryGetComponent<StepSurface>(out StepSurface surface))
            {
                SurfaceStepSounds config = _config.StepSounds.FirstOrDefault(config => config.Type == surface.Type);
                _audioSource.PlayOneShot(config.Sounds[UnityEngine.Random.Range(0, config.Sounds.Length - 1)]);
            }
    }
}

[CreateAssetMenu(fileName = "StepConfig")]
public class StepConfig : ScriptableObject
{
    public AudioMixerGroup Group;
    public SurfaceStepSounds[] StepSounds;
}

[Serializable]
public struct SurfaceStepSounds
{
    public SurfaceType Type;
    public AudioClip[] Sounds;
}

public enum SurfaceType
{
    None,
    Wood,
    Snow
}