using UnityEngine;
using System.Collections;
public class ClickableObject : MonoBehaviour
{
    [SerializeField] private string _clickTag;
    [Header("Reactive Animation")]
    [SerializeField] private bool _useReactiveAnimation;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private float _animationSpeed = 20f;
    [SerializeField] private float _animationAmplitude = 1f;

    public string ReturnTag()
    {
        return _clickTag;
    }

    public void PlayReactiveAnimation()
    {
        if (_useReactiveAnimation)
            StartCoroutine(WobbleAnimation(_animationDuration, _animationSpeed, _animationAmplitude));
    }

    private IEnumerator WobbleAnimation(float duration, float speed, float amplitude)
    {
        Vector3 startPos = transform.localPosition;
        Vector3 startScale = transform.localScale;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            float fade = Mathf.Sin(t * Mathf.PI);
            float wobble = Mathf.Sin(time * speed) * 0.6f + Mathf.Sin(time * speed * 1.7f) * 0.4f;
            float squash = 1f + wobble * 0.03f * fade * amplitude;

            transform.localScale = startScale * squash;

            yield return null;
        }
    }

}
