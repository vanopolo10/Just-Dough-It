#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class LocalizationBuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        const string prefabPath = "Assets/Resources/LocalizationManager.prefab";

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogWarning(
                "[LocalizationBuildPreprocessor] Prefab not found at path: "
                + prefabPath + ". JSON files will NOT be regenerated."
            );
            return;
        }

        var manager = prefab.GetComponent<LocalizationManager>();
        if (manager == null)
        {
            Debug.LogWarning(
                "[LocalizationBuildPreprocessor] LocalizationManager component not found on prefab: "
                + prefabPath + "."
            );
            return;
        }

        Debug.Log("[LocalizationBuildPreprocessor] Saving localization tables to JSON before build...");
        manager.SaveTablesIntoFiles();
    }
}

#endif