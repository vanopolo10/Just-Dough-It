using UnityEngine;

public class StepSurface : MonoBehaviour
{
    [SerializeField] private SurfaceType _type;
    public SurfaceType Type => _type;
}