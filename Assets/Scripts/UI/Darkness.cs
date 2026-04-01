using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Darkness : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.5f;

    private Image _image;
    private Coroutine _coroutine;
    
    public bool IsFading { get; private set; }
    public static Darkness Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _image = GetComponent<Image>();
        _image.color = new Color(0, 0, 0, 0);
        _image.enabled = false;
    }
    
    public void FadeIn(float time)
    {
        StartFade(1f, time);
    }

    public void FadeOut(float time)
    {
        StartFade(0f, time);
    }

    public void SetDark()
    {
        Stop();
        _image.enabled = true;
        _image.color = new Color(0, 0, 0, 1f);
    }

    public void SetLight()
    {
        Stop();
        _image.color = new Color(0, 0, 0, 0f);
        _image.enabled = false;
    }

    public bool IsDark()
    {
        return _image.enabled && _image.color.a >= 0.99f;
    }

    private void StartFade(float target, float time)
    {
        if (_coroutine != null) StopCoroutine(_coroutine);

        IsFading = true;
        _coroutine = StartCoroutine(FadeRoutine(target, time));
    }

    private IEnumerator FadeRoutine(float target, float fadeTime)
    {
        if (fadeTime == 0)
            fadeTime = _fadeDuration;
        
        _image.enabled = true;

        float start = _image.color.a;
        float time = 0f;

        while (time < fadeTime)
        {
            float t = time / fadeTime;
            float a = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            _image.color = new Color(0, 0, 0, a);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        _image.color = new Color(0, 0, 0, target);

        if (target == 0f)
            _image.enabled = false;

        IsFading = false;
    }

    private void Stop()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        IsFading = false;
    }
}