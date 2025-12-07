using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct CustomerQuery
{
    public Query Query;
    
    [TextArea(3, 10)]
    public string QueryText, DeclineText, AcceptText;
}

public class CustomerModel : MonoBehaviour
{
    private static readonly int StartID = Animator.StringToHash("Start");
    private static readonly int DeclineID = Animator.StringToHash("Decline");
    private static readonly int FinishID = Animator.StringToHash("Finish");
    
    [SerializeField] private Animator _animator;
    [SerializeField] private List<CustomerQuery> _queries;
    [SerializeField] private float _initDelay = 2f;
    [SerializeField] private float _finishDelay = 1f;
    [SerializeField] private float _refreshDelay = 3f;
    
    private GameObject _textBubble;
    private TextMeshProUGUI _textField; 
    private CustomerQuery _currentQuery;
    
    public CustomerQuery CurrentQuery => _currentQuery;
    
    public void Begin()
    {
        _textField = _textBubble.GetComponentInChildren<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
        _animator.SetTrigger(StartID);
        _currentQuery = _queries[UnityEngine.Random.Range(0, _queries.Count)];
        Invoke(nameof(ShowQuery), _initDelay);
    }
    
    public void ShowQuery() 
    { 
        _textField.text = _currentQuery.QueryText;
        _textBubble.SetActive(true);
    }
    
    public void Decline()
    {
        _animator.SetTrigger(DeclineID);
        _textField.text = _currentQuery.DeclineText;
        Invoke(nameof(ShowQuery), _refreshDelay);
    }
    
    public void Finish()
    {
        _animator.SetTrigger(FinishID);
        _textField.text = _currentQuery.AcceptText;
        Invoke(nameof(HideBubble), _finishDelay);
    }
    
    public void HideBubble()
    {
        _textBubble.SetActive(false);
    }
    
    public void Despawn()
    {
        Destroy(gameObject);
    }

    public void SetTextBubble(GameObject speechBubble)
    {
        _textBubble = speechBubble;
    }
}
