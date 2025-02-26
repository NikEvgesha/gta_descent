using UnityEngine;
using YG;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    [SerializeField] private LocalizationData localizationData;
    [SerializeField] private string currentLanguage;

    public LocalizationData LocalizationData => localizationData;  // ????????? ???????? ??? ???????
    public string CurrentLanguage => currentLanguage;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {

        YG2.onSwitchLang += OnSwitchLanguage;
        OnSwitchLanguage(YG2.lang);
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= OnSwitchLanguage;
    }

    public void ChangeLanguage(string newLanguage)
    {
        if (localizationData != null && localizationData.Languages.Contains(newLanguage))
        {
            currentLanguage = newLanguage;
            foreach (LocalizedText text in FindObjectsOfType<LocalizedText>())
            {
                text.SetLanguage(newLanguage);
            }
        }
    }

    private void OnValidate()
    {
        if (localizationData != null && !localizationData.Languages.Contains(currentLanguage))
        {
            currentLanguage = localizationData.Languages.Count > 0 ? localizationData.Languages[0] : "";
        }
    }

    private void OnSwitchLanguage(string langCode)
    {
        ChangeLanguage(char.ToUpper(langCode[0]) + langCode.Substring(1));
    }
}
