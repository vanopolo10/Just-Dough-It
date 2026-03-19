using UnityEngine;

public class FrostInput3D : MonoBehaviour, IFrostInput
{
    [SerializeField] private Camera _camera; 
    public bool TryGetUv(out Vector2 uv)
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray,out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                uv = hit.textureCoord;
                Debug.Log(uv);
                return true;
            }
        }
        Debug.LogWarning("Raycast did not hit window.");
        uv = Vector2.zero;
        return false;
    }
}
