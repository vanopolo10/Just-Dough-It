using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerfectComboZone : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private CraftZone _generalCraftZone;
    
    public UnityEvent OnClick;

    private void Awake()
    {
        _generalCraftZone = transform.parent.gameObject.GetComponentInChildren<CraftZone>();
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
        _generalCraftZone.AddComboClick(true);
        OnClick?.Invoke();
    }
}