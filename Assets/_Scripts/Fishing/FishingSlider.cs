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

    public void StartSlider(Action<bool> callback)
    {
        gameObject.SetActive(true);
        value = 0f;
        slider.value = value;
        movingRight = true;
        isHolding = true;
        onResult = callback;
        RandomizeGreenZone();
    }

    private void RandomizeGreenZone()
    {
        float parentWidth = sliderHandleArea.rect.width;
        float randomWidth = UnityEngine.Random.Range(greenZoneMinWidth, greenZoneMaxWidth);
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