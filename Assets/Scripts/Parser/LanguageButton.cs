using UnityEngine;
using YG;

public class LanguageButton : MonoBehaviour
{
    // Переменная, хранящая название языка.
    [SerializeField] private string language;

    // Свойство для доступа к переменной языка.
    public string Language { get => language; set => language = value; }

    // Метод вызывается при нажатии кнопки.
    public void OnButtonClick()
    {
        if (string.IsNullOrEmpty(language))
        {
            Debug.LogWarning("Переменная языка не задана!");
            return;
        }

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.ChangeLanguage(language); 
            string formattedLang = char.ToLowerInvariant(language[0]) + language.Substring(1);
            YG2.SwitchLanguage(formattedLang);
            Debug.LogFormat("Выбранный язык: {0}", language);

        }
        else
        {
            Debug.LogWarning("LocalizationManager не найден в сцене!");
        }
    }


}
