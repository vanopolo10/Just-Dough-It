using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshCollider), typeof(MeshRenderer))]
public class DoughButton : MonoBehaviour
{
    [SerializeField] private UnityEvent _actions = new();

    private AudioSource _audioSource;
    private Material _material;
    private float _occlusionTarget = 0f;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        _actions?.Invoke();
        transform.localScale = new(1.3f, 0.2f, 1.3f);
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.Play();
    }

    private void FixedUpdate()
    {
        _material.SetFloat("_OcclusionStrength", Mathf.Lerp(_material.GetFloat("_OcclusionStrength"),_occlusionTarget, 0.2f));
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.2f);
    }

    private void OnMouseEnter()
    {
        _occlusionTarget = 0.4f;
    }

    private void OnMouseExit()
    {
        _occlusionTarget = 0f;
    }
}
