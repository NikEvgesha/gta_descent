using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Data")]
public class LocalizationData : ScriptableObject
{
    [SerializeField] private List<string> languages = new List<string>();
    [SerializeField] private List<LocalizationEntry> entries = new List<LocalizationEntry>();
    private Dictionary<string, LocalizationEntry> entryDictionary;

    public List<string> Languages => languages;
    public List<LocalizationEntry> Entries => entries;


    public string GetTranslation(string key, string language)
    {
        if (entryDictionary.TryGetValue(key, out var entry))
        {
            int langIndex = languages.IndexOf(language);
            if (langIndex >= 0 && langIndex < entry.Translations.Count)
                return entry.Translations[langIndex];
        }
        return key;
    }

    public void SetData(List<string[]> rawData)
    {
        entries.Clear();
        entryDictionary = new Dictionary<string, LocalizationEntry>();

        for (int i = 1; i < rawData.Count; i++)
        {
            var row = rawData[i];
            if (row.Length < 1) continue;

            LocalizationEntry entry = new LocalizationEntry { Key = row[0].Trim('\"') };

            for (int j = 1; j < row.Length && j - 1 < languages.Count; j++)
            {
                entry.Translations.Add(row[j].Trim('\"'));
            }

            entries.Add(entry);
            entryDictionary[entry.Key] = entry;
        }
    }

}

[System.Serializable]
public class LocalizationEntry
{
    public string Key;
    public List<string> Translations = new List<string>();
}