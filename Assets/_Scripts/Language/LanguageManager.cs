using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public enum Language
    {
        Vietnamese,
        English
    }

    // Static lưu lại ngôn ngữ đã chọn
    public static Language SavedLanguage = Language.Vietnamese;

    public Language currentLanguage = Language.Vietnamese;

    private Dictionary<string, string> localizedText = new Dictionary<string, string>();

    [SerializeField] private TextAsset csvFile; // gán file Languages.csv

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load từ PlayerPrefs nếu có
            if (PlayerPrefs.HasKey("GameLanguage"))
            {
                SavedLanguage = (Language)PlayerPrefs.GetInt("GameLanguage");
            }

            currentLanguage = SavedLanguage;
            LoadLanguage(currentLanguage);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLanguage(Language lang)
    {
        localizedText.Clear();

        string[] lines = csvFile.text.Split('\n');
        if (lines.Length <= 1) return;

        int langIndex = 1; // Vietnamese mặc định
        if (lang == Language.English) langIndex = 2;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = ParseCsvLine(lines[i]); // dùng parser mới

            if (cols.Length <= langIndex) continue;

            string key = cols[0].Trim();
            string value = cols[langIndex].Trim();

            if (!localizedText.ContainsKey(key))
                localizedText.Add(key, value);
        }
    }

    public string GetText(string key)
    {
        if (localizedText.ContainsKey(key))
            return localizedText[key];
        return key;
    }

    public void ChangeLanguage(Language lang)
    {
        currentLanguage = lang;
        SavedLanguage = lang;

        // Lưu vào PlayerPrefs để không bị mất khi tắt game
        PlayerPrefs.SetInt("GameLanguage", (int)lang);
        PlayerPrefs.Save();

        LoadLanguage(lang);

        // update toàn bộ UI có LocalizedText
        LocalizedText[] texts = FindObjectsOfType<LocalizedText>();
        foreach (var t in texts)
        {
            t.UpdateText();
        }
    }

    // --- Parser CSV có hỗ trợ dấu phẩy trong dấu nháy kép ---
    private string[] ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool insideQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '\"') // toggle trạng thái trong nháy
            {
                insideQuotes = !insideQuotes;
            }
            else if (c == ',' && !insideQuotes) // ngăn cách cột
            {
                result.Add(current.Trim());
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current.Trim());
        return result.ToArray();
    }
}
