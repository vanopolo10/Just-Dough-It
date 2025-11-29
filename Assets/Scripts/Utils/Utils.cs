using UnityEngine;

public static class Utils
{
    public static Vector3 GetMouseWorldPos(float zCord)
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCord;
        return Camera.main!.ScreenToWorldPoint(mousePoint);
    }
}
