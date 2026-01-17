using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct CustomerQuery
{
    public Query   Query;
    public string  QueryKey;
    public string  DeclineKey;
    public string  AcceptKey;
}

[RequireComponent(typeof(CustomerRouteMover))]
public class CustomerModel : MonoBehaviour
{
    private static readonly int DeclineID = Animator.StringToHash("Decline");
    private static readonly int AcceptID  = Animator.StringToHash("Accept");

    [SerializeField] private Animator _animator;
    [SerializeField] private List<CustomerQuery> _queries      = new();
    [SerializeField] private float _initDelay    = 2f;
    [SerializeField] private float _finishDelay  = 1f;
    [SerializeField] private float _refreshDelay = 3f;

    private GameObject _textBubble; 
    private TMP_Text _textField;
    private CustomerQuery _currentQuery;
    private CustomerRouteMover _routeMover;

    public CustomerQuery CurrentQuery => _currentQuery;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        _routeMover = GetComponent<CustomerRouteMover>();
    }

    public void Begin()
    {
        if (_textBubble == null)
        {
            Debug.LogError("CustomerModel: text bubble is not set. Передай объект со сцены через SetTextBubble.");
            return;
        }

        if (_textField == null)
            _textField = _textBubble.GetComponentInChildren<TMP_Text>();

        if (_queries.Count > 0)
        {
            _currentQuery = _queries[UnityEngine.Random.Range(0, _queries.Count)];
        }
        else
        {
            Debug.LogWarning("CustomerModel: queries list is empty.");
        }

        // первая реплика через задержку после подхода к стойке
        Invoke(nameof(ShowQuery), _initDelay);
    }

    public void ShowQuery()
    {
        if (_textBubble != null)
            _textBubble.SetActive(true);
    }

    public void Decline()
    {
        if (_animator != null)
            _animator.SetTrigger(DeclineID);

        Invoke(nameof(ShowQuery), _refreshDelay);
    }

    public void Finish()
    {
        if (_animator != null)
            _animator.SetTrigger(AcceptID);

        Invoke(nameof(HideBubble), _finishDelay);

        if (_routeMover != null)
            _routeMover.StartExit();
    }

    public void HideBubble()
    {
        if (_textBubble != null)
            _textBubble.SetActive(false);
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
    
    public void SetTextBubble(GameObject speechBubbleSceneObject)
    {
        _textBubble = speechBubbleSceneObject;

        if (_textBubble == null)
        {
            Debug.LogError("CustomerModel: speechBubbleSceneObject is null.");
            return;
        }

        _textBubble.SetActive(false);

        _textField = _textBubble.GetComponentInChildren<TMP_Text>();
        if (_textField == null)
            Debug.LogError("CustomerModel: TMP_Text not found in speech bubble object.");
    }
}
