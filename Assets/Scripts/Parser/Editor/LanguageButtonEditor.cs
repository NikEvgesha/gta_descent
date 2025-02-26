using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(LanguageButton))]
public class LanguageButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LanguageButton langButton = (LanguageButton)target;
        serializedObject.Update();

        EditorGUILayout.LabelField("Настройка кнопки переключения языка", EditorStyles.boldLabel);

        // Пытаемся найти LocalizationManager в сцене
        LocalizationManager manager = FindObjectOfType<LocalizationManager>();
        if (manager != null && manager.LocalizationData != null)
        {
            List<string> languages = manager.LocalizationData.Languages;
            // Находим индекс выбранного языка
            int currentIndex = languages.IndexOf(langButton.Language);
            if (currentIndex < 0) currentIndex = 0;
            // Отображаем выпадающий список
            int selectedIndex = EditorGUILayout.Popup("Язык", currentIndex, languages.ToArray());
            langButton.Language = languages[selectedIndex];
        }
        else
        {
            EditorGUILayout.HelpBox("LocalizationManager или LocalizationData не найдены в сцене!", MessageType.Warning);
            // Если нет данных, оставляем возможность ввода вручную
            langButton.Language = EditorGUILayout.TextField("Язык", langButton.Language);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
