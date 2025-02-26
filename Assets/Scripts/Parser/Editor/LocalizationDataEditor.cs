using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationData))]
public class LocalizationDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LocalizationData localization = (LocalizationData)target;

        if (localization.Languages.Count == 0 || localization.Entries.Count == 0)
        {
            EditorGUILayout.HelpBox("??? ??????. ????????? ??????????? Google Sheets!", MessageType.Warning);
            return;
        }

        // ?????????
        EditorGUILayout.LabelField("?????? ???????????", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // ??????????? ?????????? (???? + ?????)
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("????", EditorStyles.boldLabel, GUILayout.Width(150));

        foreach (var lang in localization.Languages)
        {
            EditorGUILayout.LabelField(lang, EditorStyles.boldLabel, GUILayout.Width(100));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(2);

        // ??????????? ? ?????????????? ??????? ???????????
        foreach (var entry in localization.Entries)
        {
            EditorGUILayout.BeginHorizontal();

            // ??????????? ?????
            EditorGUILayout.LabelField(entry.Key, GUILayout.Width(150));

            // ??????????? ?????????
            for (int i = 0; i < localization.Languages.Count; i++)
            {
                if (i < entry.Translations.Count)
                {
                    entry.Translations[i] = EditorGUILayout.TextField(entry.Translations[i], GUILayout.Width(100));
                }
                else
                {
                    EditorGUILayout.LabelField("-", GUILayout.Width(100));
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        if (GUILayout.Button("????????? ?????????"))
        {
            EditorUtility.SetDirty(localization);
        }
    }
}