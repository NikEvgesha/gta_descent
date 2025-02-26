using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Data")]
public class LocalizationData : ScriptableObject
{
    [SerializeField] private List<string> languages = new List<string>();
    [SerializeField] private List<LocalizationEntry> entries = new List<LocalizationEntry>();

    public List<string> Languages => languages;
    public List<LocalizationEntry> Entries => entries;

    public string GetTranslation(string key, string language)
    {
        var entry = entries.Find(e => e.Key == key);
        if (entry != null)
        {
            int langIndex = languages.IndexOf(language);
            if (langIndex >= 0 && langIndex < entry.Translations.Count)
                return entry.Translations[langIndex];
        }
        return key; // ?????????? ???????? ????, ???? ??????? ?? ??????
    }

    public void SetData(List<string[]> rawData)
    {
        if (rawData.Count < 2) return;

        // ????????? ????? ?? ?????? ?????? ???????
        var header = rawData[0];
        languages = new List<string>();
        for (int i = 0; i < header.Length; i++)
        {
            string trimmed = header[i].Trim('\"');
            languages.Add(trimmed);
        }
        // ??????? ?????? ???????, ?.?. ??? ????????? ?????
        languages.RemoveAt(0);

        // ???????????? ??????
        entries.Clear();
        for (int i = 1; i < rawData.Count; i++)
        {
            var row = rawData[i];
            if (row.Length < 1) continue;

            // ??????? ?????? ? ??????
            LocalizationEntry entry = new LocalizationEntry { Key = row[0].Trim('\"') };

            for (int j = 1; j < row.Length && j - 1 < languages.Count; j++)
            {
                // ????????? ???????? ? ??????
                entry.Translations.Add(row[j].Trim('\"'));
            }
            entries.Add(entry);
        }
    }
}

[System.Serializable]
public class LocalizationEntry
{
    public string Key;
    public List<string> Translations = new List<string>();
}