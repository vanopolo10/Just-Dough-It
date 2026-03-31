using System.Collections;
using UnityEngine;

public class ThermometrArrow : MonoBehaviour
{
    private const float MinRotation = 130f;
    private const float MaxRotation = -130f;

    [SerializeField] private Oven _oven;

    private RectTransform _rectTransform;
    private float _onePercent;
    private Coroutine _rotateCoroutine;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _onePercent = (MaxRotation - MinRotation) / 100f;
        _rectTransform.localRotation = Quaternion.Euler(0f, 0f, MinRotation);
    }

    private void OnEnable()
    {
        _oven.FirePowerChanged += RotateToPower;
    }

    private void OnDisable()
    {
        _oven.FirePowerChanged -= RotateToPower;
    }

    private void RotateToPower(int power)
    {
        float targetAngle = MinRotation + _onePercent * power;

        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);

        _rotateCoroutine = StartCoroutine(RotateSmooth(targetAngle, 1f));
    }

    private IEnumerator RotateSmooth(float targetAngle, float duration)
    {
        float startAngle = GetCurrentAngle();
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float angle = Mathf.Lerp(startAngle, targetAngle, time / duration);
            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        _rectTransform.localRotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    private float GetCurrentAngle()
    {
        float angle = _rectTransform.localEulerAngles.z;
        return angle > 180f ? angle - 360f : angle;
    }
}