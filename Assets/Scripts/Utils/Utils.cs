using UnityEditor;
using UnityEngine;

public static class Utils
{
#if UNITY_EDITOR
    public class AvatarMaker
    {
        [MenuItem("CustomTools/MakeAvatarMask")]
        private static void MakeAvatarMask()
        {
            GameObject activeGameObject = Selection.activeGameObject;

            if (activeGameObject != null)
            {
                AvatarMask avatarMask = new AvatarMask();

                avatarMask.AddTransformPath(activeGameObject.transform);

                var path = $"Assets/{activeGameObject.name.Replace(':', '_')}.mask";
                AssetDatabase.CreateAsset(avatarMask, path);
            }
        }

        [MenuItem("CustomTools/MakeAvatar")]
        private static void MakeAvatar()
        {
            GameObject activeGameObject = Selection.activeGameObject;

            if (activeGameObject != null)
            {
                Avatar avatar = AvatarBuilder.BuildGenericAvatar(activeGameObject, "");
                avatar.name = activeGameObject.name;
                Debug.Log(avatar.isHuman ? "is human" : "is generic");

                var path = $"Assets/{avatar.name.Replace(':', '_')}.ht";
                AssetDatabase.CreateAsset(avatar, path);
            }
        }
    }
#endif

    public static Vector3 GetMouseWorldPos(float zCord)
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCord;
        return Camera.main!.ScreenToWorldPoint(mousePoint);
    }
}