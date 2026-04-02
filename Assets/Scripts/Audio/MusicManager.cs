using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField, Min(0)] private float _fadeTime = 5;
    [SerializeField] private List<AudioClip> _clips = new();

    private AudioSource _audioSource;
    private float _initialVolume;

    private void Awake() => DontDestroyOnLoad(gameObject);

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _initialVolume = _audioSource.volume;
        if (_clips.Count > 0)
            StartCoroutine(Play());
        else
            Debug.LogWarning("Список музыки пустой, кина не будет");
    }

    private IEnumerator Play()
    {
        while (true)
        {
            _clips = Shuffle();
            foreach (var clip in _clips)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                yield return FadeIn(_fadeTime);
                yield return new WaitForSeconds(clip.length - _fadeTime * 2);
                yield return FadeOut(_fadeTime);
            }
            yield return null;
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        float startVolume = 0;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _audioSource.volume = Mathf.Lerp(startVolume, _initialVolume, t);
            yield return null;
        }
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = _audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        _audioSource.Stop();
    }

    private List<AudioClip> Shuffle()
    {
        List<AudioClip> shuffled = new(_clips);

        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        return shuffled;
    }
}
