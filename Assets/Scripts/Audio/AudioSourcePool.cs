using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] private GameObject _audioSourcePrefab;
    public GameObject AudioSourcePrefab => _audioSourcePrefab;
    [SerializeField] private int _poolSize;
    private List<AudioSource> _availableSources;

    void Awake()
    {
        _availableSources = new List<AudioSource>();
        
        for(int i = 0; i < _poolSize; i++)
        {
            GameObject newSourceObj = Instantiate(_audioSourcePrefab, transform);
            AudioSource newSource = newSourceObj.GetComponent<AudioSource>();

            newSourceObj.SetActive(false);
            _availableSources.Add(newSource);
        }
    }

    public AudioSource GetAudioSource()
    {
        foreach(AudioSource source in _availableSources)
        {
            if(!source.isPlaying)
            {
                source.gameObject.SetActive(true);
                return source;
            }
        }

        GameObject newSourceObj = Instantiate(_audioSourcePrefab, transform);
        AudioSource newSource = newSourceObj.GetComponent<AudioSource>();
        _availableSources.Add(newSource);

        return newSource;
    }

    public void ReturnAudioSource(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
    }
}
