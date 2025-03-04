using UnityEngine;
using System;
using System.Collections.Generic;
using YG;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] private LocalizationData localizationData;
    [SerializeField] private string currentLanguage;

    public event Action<string> OnLanguageChanged; // Событие для обновления UI
    public LocalizationData LocalizationData => localizationData;
    public string CurrentLanguage => currentLanguage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("LocalizationManager уже существует! Удаляем дубликат.");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        YG2.onSwitchLang += OnSwitchLanguage;

        if (!string.IsNullOrEmpty(YG2.lang))
        {
            OnSwitchLanguage(YG2.lang);
        }
        else if (!string.IsNullOrEmpty(currentLanguage))
        {
            ChangeLanguage(currentLanguage);
        }
        else if (localizationData != null && localizationData.Languages.Count > 0)
        {
            ChangeLanguage(localizationData.Languages[0]); // Устанавливаем первый язык по умолчанию
        }
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= OnSwitchLanguage;
    }

    public void ChangeLanguage(string newLanguage)
    {
        if (localizationData == null || !localizationData.Languages.Contains(newLanguage) || newLanguage == currentLanguage)
        {
            return;
        }

        currentLanguage = newLanguage;

        // Вызываем событие, чтобы обновить все UI-элементы
        OnLanguageChanged?.Invoke(newLanguage);

        // Обновляем все текстовые объекты
        foreach (LocalizedText text in FindObjectsOfType<LocalizedText>())
        {
            text.SetLanguage(newLanguage);
        }

        Debug.Log($"Язык изменен на: {newLanguage}");
    }

    private void OnSwitchLanguage(string langCode)
    {
        if (string.IsNullOrEmpty(langCode))
        {
            Debug.LogWarning("Получен пустой код языка!");
            return;
        }

        string formattedLang = char.ToUpper(langCode[0]) + langCode.Substring(1);
        ChangeLanguage(formattedLang);
    }
}
