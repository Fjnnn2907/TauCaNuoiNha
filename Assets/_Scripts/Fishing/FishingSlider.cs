using UnityEngine;
using UnityEngine.UI;
using System;

public class FishingSlider : MonoBehaviour
{
    [Header("References")]
    public Slider slider;
    public RectTransform greenZone;
    public RectTransform sliderHandleArea;

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
    }

    void Update()
    {
        if (!isHolding) return;

        value += (movingRight ? 1 : -1) * speed * Time.deltaTime;
        if (value >= 1f) { value = 1f; movingRight = false; }
        else if (value <= 0f) { value = 0f; movingRight = true; }

        slider.value = value;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHolding = false;
            bool inZone = CheckInGreenZone();
            onResult?.Invoke(inZone);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Gọi khi bắt đầu thanh slider, truyền callback và bonusRate để tăng độ rộng vùng xanh
    /// </summary>
    public void StartSlider(Action<bool> callback, float bonusRate)
    {
        gameObject.SetActive(true);
        value = 0f;
        slider.value = value;
        movingRight = true;
        isHolding = true;
        onResult = callback;

        RandomizeGreenZone(bonusRate);
    }

    /// <summary>
    /// Sinh vùng xanh ngẫu nhiên, và tăng độ rộng nếu bonusRate cao
    /// </summary>
    private void RandomizeGreenZone(float bonusRate)
    {
        float parentWidth = sliderHandleArea.rect.width;

        // Chuyển bonusRate thành % (giới hạn từ 0 đến 1)
        float bonusPercent = Mathf.Clamp01(bonusRate / 100f);

        // Tính độ rộng vùng xanh dựa theo bonus
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
