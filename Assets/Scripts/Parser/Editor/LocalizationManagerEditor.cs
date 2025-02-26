using UnityEditor;
using System.Collections.Generic;
using YG;

[CustomEditor(typeof(LocalizationManager))]
public class LocalizationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LocalizationManager manager = (LocalizationManager)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("localizationData"));

        if (manager.LocalizationData != null)
        {
            List<string> languages = manager.LocalizationData.Languages;
            int langIndex = languages.IndexOf(manager.CurrentLanguage);
            langIndex = EditorGUILayout.Popup("Current Language", langIndex, languages.ToArray());

            if (langIndex >= 0 && langIndex < languages.Count)
            {
                //manager.ChangeLanguage(languages[langIndex]);
                YG2.SwitchLanguage(char.ToLower(languages[langIndex][0]) + languages[langIndex].Substring(1));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
