using UnityEngine;

public class Knife : MonoBehaviour
{
    private PhysicsDrag _drag;
    [SerializeField] private Transform _raycastPoint;
    private CuttingManager _cuttingManager;
    private  LayerMask _layer;
    private bool _locked = false;

    public void Start()
    {
        _drag = GetComponent<PhysicsDrag>();
        _layer = LayerMask.GetMask("CuttingPoint");
    }

    private void OnMouseDrag()
    {
        if (!_locked 
            && Input.GetMouseButtonDown(1)
            && _drag.IsDragging
            )
        {
            Debug.Log("Knife Right click detected, performing raycast");
            RaycastHit hit;
            if (
                Physics.Raycast(new Ray(_raycastPoint.position, _raycastPoint.up * -1), out hit, 100, _layer, QueryTriggerInteraction.Collide)
                || Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _layer, QueryTriggerInteraction.Collide)
               ) 
            {
                Debug.Log($"Knife raycast successful, hit {hit.collider.name}");
                _cuttingManager = hit.collider.GetComponentInParent<CuttingManager>();

                if (_cuttingManager == null) return;

                LockToPoint(_cuttingManager.GetLockPoint());
                _cuttingManager.StartCutting(this);
            }
        }
        else
        {
            //Debug.Log("Knife click detected, but conditions not met for raycast");
            //Debug.Log($"_locked: {_locked}, Right Click: {Input.GetMouseButton(1)}, IsDragging: {_drag.IsDragging}");
        }
    }


    private void LockToPoint(Transform targetPoint) {
        _locked = true;
        _drag.Override(targetPoint, true);
        _drag.SetLocked(true);
    }
    private void Unlock()
    {
        _locked = false;
        _drag.CancelOverride();
        _drag.SetLocked(false);
    }
    public void FinishCutting() {
        Unlock();
        _drag.TryStartDragging();
    }
}
