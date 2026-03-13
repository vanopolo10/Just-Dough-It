using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Darkness : MonoBehaviour
{
    [SerializeField] private float _blinkDuration = 2f;
    [SerializeField] private float _fullDarkDelay = 1.5f;
    
    private Image _darkness;

    public event Action Darkened; 
    
    private void Awake()
    {
        _darkness = GetComponent<Image>();
        _darkness.color = new Color(0, 0, 0, 0);
        _darkness.enabled = false;
    }

    public void WakeUp()
    {
        StopAllCoroutines();
        _darkness.enabled = true;
        _darkness.color = new Color(0, 0, 0, 1f);
        StartCoroutine(Fade(false));
    }
    
    public void FallAsleep()
    {
        StopAllCoroutines();
        _darkness.enabled = true;
        _darkness.color = new Color(0, 0, 0, 0f);
        StartCoroutine(Fade(true));
    }
    
    public void Blink()
    {
        StopAllCoroutines();
        _darkness.enabled = true;
        StartCoroutine(BlinkAnimation());
    }

    private IEnumerator BlinkAnimation()
    {
        yield return Fade(true);
        
        _darkness.color = new Color(0, 0, 0, 1f);
        
        yield return new WaitForSeconds(_fullDarkDelay);

        yield return Fade(false);
        
        _darkness.enabled = false;
    }

    private IEnumerator Fade(bool isFadingIn)
    {
        float elapsedTime = 0f;
        float fadeDuration = _blinkDuration / 2f;
        
        float startAlpha = isFadingIn ? 0f : 1f;
        float endAlpha = isFadingIn ? 1f : 0f;
        
        _darkness.enabled = true;
        
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            _darkness.color = new Color(0, 0, 0, alpha);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        _darkness.color = new Color(0, 0, 0, endAlpha);

        if (!isFadingIn)
            _darkness.enabled = false;
        else
            Darkened?.Invoke();
    }
}