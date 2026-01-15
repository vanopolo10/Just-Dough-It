using UnityEngine;

public class DragMagnet : MonoBehaviour
{
    [SerializeField] private PhysicsDrag _target;

    //private Queue<PhysicsDrag> _targetQueue = new Queue<PhysicsDrag>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Draggable")) return;
        if (!other.GetComponent<PhysicsDrag>().IsDragging) return;
        
        if (_target == null)
            _target = other.GetComponent<PhysicsDrag>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Draggable")) return;
        
        if (other.GetComponent<PhysicsDrag>().IsDragging) _target = null;
    }

    private void OnMouseEnter()
    {
        if (_target == null) return;
        
        Debug.Log("mouse entered magnet, target: " + _target.name);
        
        if (_target.IsDragging)
            _target.Override(transform);
    }

    private void OnMouseExit()
    {
        if (_target == null)
            return;
            
        Debug.Log("mouse exited magnet, target: " + _target.name);
        
        if (_target.IsDragging)
            _target.CancelOverride();
    }

    private void OnMouseDown()
    {
        if (_target != null)
            _target.StartDragging();
    }
}