using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizationManager))]
public class LocalizationEditor : Editor
{
    private LocalizationManager _manager;
    private Vector2 _scroll;

    private void OnEnable()
    {
        _manager = (LocalizationManager)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (_manager.Tables == null || _manager.Tables.Count == 0)
        {
            EditorGUILayout.HelpBox("Нет таблиц локализации", MessageType.Info);

            if (!GUILayout.Button("Добавить таблицу")) return;

            _manager.Tables?.Add(new LocalizationTable
            {
                Code = "new",
                Keys = new List<KeyPair>()
            });

            EditorUtility.SetDirty(_manager);

            return;
        }

        DrawToolbar();
        DrawTableGrid();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Добавить язык", GUILayout.Height(22)))
        {
            Undo.RecordObject(_manager, "Add Localization Table");
            _manager.Tables.Add(new LocalizationTable()
            {
                Code = "new",
                Keys = new List<KeyPair>()
            });
            EditorUtility.SetDirty(_manager);
        }

        if (GUILayout.Button("Добавить ключ", GUILayout.Height(22)))
        {
            Undo.RecordObject(_manager, "Add Key");

            foreach (var table in _manager.Tables)
            {
                table.Keys.Add(new KeyPair()
                {
                    Key = "new_key",
                    Value = ""
                });
            }

            EditorUtility.SetDirty(_manager);
        }

        if (GUILayout.Button("Сохранить в проект", GUILayout.Height(22))) _manager.SaveTablesIntoFiles();

        if (GUILayout.Button("Загрузить из проекта", GUILayout.Height(22))) _manager.LoadTablesFromFiles();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawTableGrid()
    {
        EditorGUILayout.Space(10);

        int maxRows = _manager.Tables.Max(t => t.Keys.Count);

        if (maxRows == 0)
        {
            EditorGUILayout.HelpBox("Нет ключей", MessageType.Info);
            return;
        }

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("", GUILayout.Width(25));
        EditorGUILayout.LabelField("Key", EditorStyles.boldLabel, GUILayout.Width(150));

        foreach (var table in _manager.Tables)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(200));

            EditorGUILayout.BeginHorizontal();
            table.Code = EditorGUILayout.TextField(table.Code);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                Undo.RecordObject(_manager, "Remove Language Table");
                _manager.Tables.Remove(table);
                EditorUtility.SetDirty(_manager);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();

        HashSet<string> duplicateKeys = new();

        foreach (var table in _manager.Tables)
        {
            var dups = table.Keys.GroupBy(k => k.Key).Where(g => g.Count() > 1 && !string.IsNullOrEmpty(g.Key)).Select(g => g.Key);

            foreach (var d in dups) duplicateKeys.Add(d);
        }

        for (int row = 0; row < maxRows; row++)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                Undo.RecordObject(_manager, "Remove Key");

                foreach (var table in _manager.Tables)
                {
                    if (row < table.Keys.Count) table.Keys.RemoveAt(row);
                }

                EditorUtility.SetDirty(_manager);
                EditorGUILayout.EndHorizontal();
                break;
            }

            string key = _manager.Tables[0].Keys[row].Key;
            bool isDuplicate = duplicateKeys.Contains(key);

            Color oldColor = GUI.backgroundColor;
            if (isDuplicate) GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);

            string newKey = EditorGUILayout.TextField(key, GUILayout.Width(150));

            GUI.backgroundColor = oldColor;

            if (newKey != key)
            {
                Undo.RecordObject(_manager, "Rename Key");

                foreach (var table in _manager.Tables)
                {
                    if (row < table.Keys.Count) table.Keys[row].Key = newKey;
                }

                EditorUtility.SetDirty(_manager);
            }

            foreach (var table in _manager.Tables)
            {
                if (row >= table.Keys.Count) table.Keys.Add(new KeyPair() { Key = newKey, Value = "" });

                table.Keys[row].Value = EditorGUILayout.TextField(table.Keys[row].Value, GUILayout.Width(200));
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}
