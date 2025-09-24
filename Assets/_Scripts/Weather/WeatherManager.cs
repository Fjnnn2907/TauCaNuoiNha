using UnityEngine;
using System.Collections;

public class WeatherManager : Singleton<WeatherManager>
{
    [Header("Weather Objects")]
    public GameObject rainEffect;

    [Header("Weather State")]
    public bool isRaining = false;

    [Header("Weather Settings")]
    [Range(0f, 1f)] public float rainChance = 0.3f; // Xác suất mưa (30%)
    public Vector2 rainDurationRange = new Vector2(20f, 60f);   // Thời gian mưa (giây)
    public Vector2 clearDurationRange = new Vector2(30f, 90f);  // Thời gian tạnh (giây)

    private void Start()
    {
        StartCoroutine(WeatherCycle());
    }

    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            if (isRaining)
            {
                // Nếu đang mưa, chờ hết mưa
                float rainDuration = Random.Range(rainDurationRange.x, rainDurationRange.y);
                yield return new WaitForSeconds(rainDuration);

                string msg = LanguageManager.Instance.GetText("weather_upcoming_clear");
                NotificationManager.Instance?.ShowNotification(msg);

                yield return new WaitForSeconds(2f); // Delay 2 giây trước khi đổi
                SetWeather(false);
            }
            else
            {
                // Nếu trời quang, chờ một khoảng rồi random xem có mưa không
                float clearDuration = Random.Range(clearDurationRange.x, clearDurationRange.y);
                yield return new WaitForSeconds(clearDuration);

                if (Random.value <= rainChance) // Check tỉ lệ mưa
                {
                    string msg = LanguageManager.Instance.GetText("weather_upcoming_rain");
                    NotificationManager.Instance?.ShowNotification(msg);

                    yield return new WaitForSeconds(2f); // Delay 2 giây trước khi đổi
                    SetWeather(true);
                }
            }
        }
    }

    public void SetWeather(bool raining)
    {
        isRaining = raining;

        if (rainEffect != null)
            rainEffect.SetActive(raining);

        if (raining)
            AudioManager.Instance.PlaySFX("RainLoop", true); // bật loop
        else
            AudioManager.Instance.StopSFX(); // dừng loop

        Debug.Log("Weather Changed: " + (raining ? "🌧️ Mưa" : "☀️ Quang đãng"));
    }

    [ContextMenu("Test Rain")]
    public void ActiveRain()
        => SetWeather(true);

    public bool IsRaining() => isRaining;
}
