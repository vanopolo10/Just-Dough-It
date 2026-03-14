using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueBubble : MonoBehaviour, IPointerClickHandler
{
    public event Action OnBubbleClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[DialogueBubble] Bubble clicked!");
        OnBubbleClicked?.Invoke();
    }
}