using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class FishingSlider : MonoBehaviour
{
    [Header("References")]
    public Slider slider;
    public RectTransform greenZone;
    public RectTransform sliderHandleArea;

    [Header("Button")]
    public Button castButton;
    public TextMeshProUGUI castButtonText;

    [Header("Settings")]
    public float speed = 1f;
    public float greenZoneMinWidth = 30f;
    public float greenZoneMaxWidth = 100f;

    private float value = 0f;
    private bool movingRight = true;
    private bool isHolding = false;
    private Action<bool> onResult;

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        gameObject.SetActive(false);

        if (castButton != null)
            castButton.onClick.AddListener(OnCastButtonPressed);

        // 🔑 Set text mặc định khi khởi động
        if (castButtonText != null)
            castButtonText.text = LanguageManager.Instance.GetText("cau");
    }

    void Update()
    {
        if (!isHolding) return;

        value += (movingRight ? 1 : -1) * speed * Time.deltaTime;
        if (value >= 1f) { value = 1f; movingRight = false; }
        else if (value <= 0f) { value = 0f; movingRight = true; }

        slider.value = value;
    }

    public void StartSlider(Action<bool> callback, float bonusRate)
    {
        gameObject.SetActive(true);
        value = 0f;
        slider.value = value;
        movingRight = true;
        isHolding = true;
        onResult = callback;

        RandomizeGreenZone(bonusRate);

        if (castButton != null && castButtonText != null)
        {
            // 🔑 Khi bắt đầu → "Thả"
            castButtonText.text = LanguageManager.Instance.GetText("tha");
        }
    }

    private void OnCastButtonPressed()
    {
        if (!isHolding) return;

        isHolding = false;
        bool inZone = CheckInGreenZone();
        onResult?.Invoke(inZone);
        gameObject.SetActive(false);

        if (castButton != null && castButtonText != null)
        {
            // 🔑 Khi nhấn xong → "Câu"
            castButtonText.text = LanguageManager.Instance.GetText("cau");
        }
    }

    private void RandomizeGreenZone(float bonusRate)
    {
        float parentWidth = sliderHandleArea.rect.width;
        float bonusPercent = Mathf.Clamp01(bonusRate / 100f);
        float effectiveMaxWidth = Mathf.Lerp(greenZoneMinWidth, greenZoneMaxWidth, bonusPercent);
        float randomWidth = UnityEngine.Random.Range(greenZoneMinWidth, effectiveMaxWidth);

        float maxStart = parentWidth - randomWidth;
        float randomStart = UnityEngine.Random.Range(0f, maxStart);

        greenZone.sizeDelta = new Vector2(randomWidth, greenZone.sizeDelta.y);
        greenZone.anchoredPosition = new Vector2(randomStart, greenZone.anchoredPosition.y);
    }

    private bool CheckInGreenZone()
    {
        float handlePos = value * sliderHandleArea.rect.width;
        float greenStart = greenZone.anchoredPosition.x;
        float greenEnd = greenStart + greenZone.rect.width;

        return handlePos >= greenStart && handlePos <= greenEnd;
    }
}
