using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LocalizedText localizedText = (LocalizedText)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("localizationData"));

        if (localizedText.LocalizationData != null)
        {
            List<string> keys = new List<string>();
            foreach (var entry in localizedText.LocalizationData.Entries)
            {
                keys.Add(entry.Key);
            }

            int selectedIndex = keys.IndexOf(localizedText.SelectedKey);
            selectedIndex = EditorGUILayout.Popup("Localization Key", selectedIndex, keys.ToArray());

            if (selectedIndex >= 0 && selectedIndex < keys.Count)
            {
                localizedText.SelectedKey = keys[selectedIndex];
            }

            List<string> languages = localizedText.LocalizationData.Languages;
            int langIndex = languages.IndexOf(localizedText.CurrentLanguage);
            langIndex = EditorGUILayout.Popup("Current Language", langIndex, languages.ToArray());

            if (langIndex >= 0 && langIndex < languages.Count)
            {
                localizedText.CurrentLanguage = languages[langIndex];
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
