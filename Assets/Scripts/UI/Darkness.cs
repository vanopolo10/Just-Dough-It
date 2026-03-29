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

    public void FadeIn()
    {
        StartFade(1f);
    }

    public void FadeOut()
    {
        StartFade(0f);
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

    private void StartFade(float target)
    {
        if (_coroutine != null) StopCoroutine(_coroutine);

        IsFading = true;
        _coroutine = StartCoroutine(FadeRoutine(target));
    }

    private IEnumerator FadeRoutine(float target)
    {
        _image.enabled = true;

        float start = _image.color.a;
        float time = 0f;

        while (time < _fadeDuration)
        {
            float t = time / _fadeDuration;
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