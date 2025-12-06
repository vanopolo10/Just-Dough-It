using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct CustomerQuery
{
    public Query query;
    public string queryText, DeclineText, AcceptText;
}

public class CustomerModel : MonoBehaviour
{
    private Animator animator;
    public CustomerQuery currentQuery;
    [SerializeField] private List<CustomerQuery> queries;
    [SerializeField] private float initDelay = 2f, finishDelay = 1f, refreshDelay = 3f;
    public TextMeshProUGUI textField;
    public GameObject textBubble;

    public void Begin()
    {
        textField = textBubble.GetComponentInChildren<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("Start");
        currentQuery = queries[UnityEngine.Random.Range(0, queries.Count)];
        Invoke("ShowQuery", initDelay);
    }
    public void ShowQuery() 
    { 
        textField.text = currentQuery.queryText;
        textBubble.SetActive(true);
    }
    public void Decline()
    {
        animator.SetTrigger("Decline");
        textField.text = currentQuery.DeclineText;
        Invoke("ShowQuery", refreshDelay);
    }
    public void Finish()
    {
        animator.SetTrigger("Finish");
        textField.text = currentQuery.AcceptText;
        Invoke("HideBubble", finishDelay);
    }
    public void HideBubble()
    {
        textBubble.SetActive(false);
    }
    public void Despawn()
    {
        Destroy(gameObject);
    }
}
