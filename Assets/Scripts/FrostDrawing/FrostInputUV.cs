using UnityEngine;

public class FrostInputUV : MonoBehaviour, IFrostInput
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Camera _camera;
    
    public bool TryGetUv(out Vector2 uv)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, _camera, out var localPoint))
        { 
            uv = new Vector2(
                (localPoint.x - _rectTransform.rect.x) / _rectTransform.rect.width,
                (localPoint.y - _rectTransform.rect.y) / _rectTransform.rect.height);
            return true;
        }
        uv = new Vector2(-1, -1);
        return false;
    }
}
