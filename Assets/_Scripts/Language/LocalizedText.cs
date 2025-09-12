using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string key; // Key trong file CSV
    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if(key == "")
            return;
        if (LanguageManager.Instance != null)
        {
            textComponent.text = LanguageManager.Instance.GetText(key);
        }
    }

    // Cho phép đổi key lúc runtime
    public void SetKey(string newKey)
    {
        key = newKey;
        UpdateText();
    }
}
