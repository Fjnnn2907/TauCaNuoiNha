// LanguageManager.cs
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public enum Language
    {
        Vietnamese,
        English
    }

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

        if (csvFile == null || string.IsNullOrEmpty(csvFile.text))
        {
            Debug.LogWarning("LanguageManager: csvFile is null or empty");
            return;
        }

        List<string[]> records = ParseCsvRecords(csvFile.text);

        if (records == null || records.Count <= 1)
        {
            Debug.LogWarning("LanguageManager: no records found in CSV");
            return;
        }

        int langIndex = 1; // mặc định cột 1 = Vietnamese
        if (lang == Language.English) langIndex = 2;

        for (int i = 1; i < records.Count; i++) // bắt đầu từ 1 (bỏ header)
        {
            var cols = records[i];
            if (cols.Length == 0) continue;

            string key = cols[0]?.Trim();
            if (string.IsNullOrEmpty(key)) continue;

            string value = "";
            if (cols.Length > langIndex)
                value = cols[langIndex] ?? "";

            if (!localizedText.ContainsKey(key))
                localizedText.Add(key, value);
            else
                localizedText[key] = value; // ghi đè nếu trùng
        }
    }

    public string GetText(string key)
    {
        if (localizedText.TryGetValue(key, out var val))
            return val;
        return key;
    }

    public void ChangeLanguage(Language lang)
    {
        currentLanguage = lang;
        SavedLanguage = lang;
        PlayerPrefs.SetInt("GameLanguage", (int)lang);
        PlayerPrefs.Save();

        LoadLanguage(lang);

        LocalizedText[] texts = FindObjectsOfType<LocalizedText>();
        foreach (var t in texts)
            t.UpdateText();
    }

    // -------------------------
    // Parser CSV: hỗ trợ:
    //  - trường có dấu phẩy
    //  - trường có ngắt dòng (newline) bên trong (phải được bao trong "")
    //  - escape dấu nháy kép bằng ""
    // -------------------------
    private List<string[]> ParseCsvRecords(string text)
    {
        var records = new List<string[]>();
        var currentRecord = new List<string>();
        var field = new StringBuilder();
        bool insideQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '"')
            {
                // nếu là "" (escaped quote) -> append một " vào field và skip next "
                if (insideQuotes && i + 1 < text.Length && text[i + 1] == '"')
                {
                    field.Append('"');
                    i++; // skip the escaped quote
                }
                else
                {
                    // toggle trạng thái trong/ngoài quotes
                    insideQuotes = !insideQuotes;
                }
            }
            else if (c == ',' && !insideQuotes)
            {
                // kết thúc field
                currentRecord.Add(field.ToString());
                field.Clear();
            }
            else if (c == '\r')
            {
                // ignore CR (Windows CRLF handling)
            }
            else if (c == '\n' && !insideQuotes)
            {
                // kết thúc record
                currentRecord.Add(field.ToString());
                field.Clear();
                records.Add(currentRecord.ToArray());
                currentRecord = new List<string>();
            }
            else
            {
                // ký tự bình thường (cũng bao gồm newline nếu insideQuotes)
                field.Append(c);
            }
        }

        // thêm phần còn lại (nếu file không kết thúc bằng newline)
        if (field.Length > 0 || currentRecord.Count > 0)
        {
            currentRecord.Add(field.ToString());
            records.Add(currentRecord.ToArray());
        }

        // Có thể file có BOM ở đầu, loại bỏ nếu có
        if (records.Count > 0 && records[0].Length > 0)
        {
            records[0][0] = records[0][0].TrimStart('\uFEFF');
        }

        // Trim các field (loại bỏ khoảng trắng đầu/cuối)
        for (int r = 0; r < records.Count; r++)
        {
            for (int c = 0; c < records[r].Length; c++)
            {
                if (records[r][c] != null)
                    records[r][c] = records[r][c].Trim();
            }
        }

        return records;
    }
}
