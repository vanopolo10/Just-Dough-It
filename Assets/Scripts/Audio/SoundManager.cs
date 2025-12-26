using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    public static readonly AudioEventsData AudioEvent = new AudioEventsData();
    public class AudioEventsData
    {
        public UnityAction<AudioClip, float, float> OnPlaySound;
        public UnityAction<AudioClip, float, float, Vector3> OnPlaySoundIn3D;
    }


    [SerializeField] private AudioSourcePool _sourcePool;

    private AudioRolloffMode _defaulRollofMode;

    private void OnEnable()
    {
        AudioEvent.OnPlaySound += PlaySound;
        AudioEvent.OnPlaySoundIn3D += PlaySoundIn3D;
    }


    private void OnDisable()
    {
        AudioEvent.OnPlaySound -= PlaySound;
        AudioEvent.OnPlaySoundIn3D -= PlaySoundIn3D;
    }

    private void Start()
    {
        _defaulRollofMode = _sourcePool.AudioSourcePrefab.GetComponent<AudioSource>().rolloffMode;
    }


    public void PlaySound(AudioClip clip, float volume, float pitch)
    {
        AudioSource source = _sourcePool.GetAudioSource();
        if (source != null)
        {
            source.pitch = pitch;
            source.volume = volume;
            source.clip = clip;
            source.Play();
            source.spatialBlend = 0;
            
            StartCoroutine(ReturnToPoolAfterPlaying(source));
        }
    }
    public void PlaySoundIn3D(AudioClip clip, float volume, float pitch, Vector3 position)
    {
        AudioSource source = _sourcePool.GetAudioSource();

        if (source != null)
        {
            source.volume = volume;
            source.gameObject.transform.position = position;
            source.pitch = pitch;
            source.maxDistance = 100;
            source.clip = clip;
            source.Play();

            StartCoroutine(ReturnToPoolAfterPlaying(source));
        }
    }


    private System.Collections.IEnumerator ReturnToPoolAfterPlaying(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        _sourcePool.ReturnAudioSource(source);
        source.rolloffMode = _defaulRollofMode;
        source.pitch = 1;
        source.spatialBlend = 1;
        source.transform.position = Vector3.zero;
    }
}