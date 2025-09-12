using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguageUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button viButton;
    public Button enButon;
    public GameObject tickVietnamese; // Icon dấu tích VN
    public GameObject tickEnglish;    // Icon dấu tích EN

    private void Start()
    {
        UpdateUI();

        viButton.onClick.AddListener(SetVietnamese);
        enButon.onClick.AddListener(SetEnglish);
    }
    

    public void SetVietnamese()
    {
        LanguageManager.Instance.ChangeLanguage(LanguageManager.Language.Vietnamese);
        UpdateUI();
    }

    public void SetEnglish()
    {
        LanguageManager.Instance.ChangeLanguage(LanguageManager.Language.English);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (LanguageManager.Instance.currentLanguage == LanguageManager.Language.Vietnamese)
        {
            tickVietnamese.SetActive(true);
            tickEnglish.SetActive(false);
        }
        else
        {
            tickVietnamese.SetActive(false);
            tickEnglish.SetActive(true);
        }
    }
}
