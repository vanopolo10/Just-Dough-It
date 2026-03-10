using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerfectComboZone : MonoBehaviour, IPointerDownHandler
{
    private CraftZone _craftZone;
    public UnityEvent OnClick;

    private void Awake()
    {
        _craftZone = transform.parent.gameObject.GetComponentInChildren<CraftZone>();
    }

    private void RemoveZone()
    {
        GetComponent<GraphicRaycaster>().enabled = false;
        print("perfect Combo zone removed");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        print("perfect combo zone clicked");
        RemoveZone();
        _craftZone.AddComboClick(true);
        OnClick?.Invoke();
    }
}