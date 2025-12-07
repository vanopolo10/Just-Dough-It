using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct CustomerQuery
{
    public Query Query;
    
    public string QueryKey, DeclineKey, AcceptKey;
}

public class CustomerModel : MonoBehaviour
{
    //private static readonly int StartID = Animator.StringToHash("Start");
    private static readonly int DeclineID = Animator.StringToHash("Decline");
    private static readonly int AcceptID = Animator.StringToHash("Accept");
    
    [SerializeField] private Animator _animator;
    [SerializeField] private List<CustomerQuery> _queries;
    [SerializeField] private float _initDelay = 2f;
    [SerializeField] private float _finishDelay = 1f;
    [SerializeField] private float _refreshDelay = 3f;
    
    private GameObject _textBubble;
    private TMP_Text _textField; 
    private CustomerQuery _currentQuery;
    
    public CustomerQuery CurrentQuery => _currentQuery;
    
    public void Begin()
    {
        _textField = _textBubble.GetComponentInChildren<TMP_Text>();
        _animator = GetComponent<Animator>();
//        _currentQuery = _queries[UnityEngine.Random.Range(0, _queries.Count)];
        Invoke(nameof(ShowQuery), _initDelay);

        LocalizationManager.Instance.OnLanguageChange += UpdateQueryText;
    }
    
    private void UpdateQueryText()
    {
        _textField.text = LocalizationManager.Instance.SelectedTable.GetPair(_currentQuery.QueryKey);
    }

    public void ShowQuery() 
    {
        UpdateQueryText();
        _textBubble.SetActive(true);
    }
    
    public void Decline()
    {
        _animator.SetTrigger(DeclineID);
        _textField.text = LocalizationManager.Instance.SelectedTable.GetPair(_currentQuery.DeclineKey);
        Invoke(nameof(ShowQuery), _refreshDelay);
    }
    
    public void Finish()
    {
        _animator.SetTrigger(AcceptID);
        _textField.text = LocalizationManager.Instance.SelectedTable.GetPair(_currentQuery.AcceptKey);
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
