using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class WeatherManager : Singleton<WeatherManager>
{
    [Header("Weather Objects")]
    public GameObject rainEffect;
    public Light2D globalLight;

    [Header("Weather State")]
    public bool isRaining = false;

    [Header("Weather Settings")]
    [Range(0f, 1f)] public float rainChance = 0.3f; 
    public Vector2 rainDurationRange = new Vector2(20f, 60f); 
    public Vector2 clearDurationRange = new Vector2(30f, 90f);  

    [Header("Lighting Settings")]
    public Vector2 rainLightRange = new Vector2(0.3f, 0.5f);
    public float lightTransitionDuration = 2f;

    private float defaultLightIntensity;

    private void Start()
    {
        if (globalLight != null)
            defaultLightIntensity = globalLight.intensity;

        StartCoroutine(WeatherCycle());
    }

    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            if (isRaining)
            {
                float rainDuration = Random.Range(rainDurationRange.x, rainDurationRange.y);
                yield return new WaitForSeconds(rainDuration);

                string msg = LanguageManager.Instance.GetText("weather_upcoming_clear");
                NotificationManager.Instance?.ShowNotification(msg);

                yield return new WaitForSeconds(2f);
                SetWeather(false);
            }
            else
            {
                float clearDuration = Random.Range(clearDurationRange.x, clearDurationRange.y);
                yield return new WaitForSeconds(clearDuration);

                if (Random.value <= rainChance)
                {
                    string msg = LanguageManager.Instance.GetText("weather_upcoming_rain");
                    NotificationManager.Instance?.ShowNotification(msg);

                    yield return new WaitForSeconds(2f);
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
        {
            AudioManager.Instance.PlaySFX("RainLoop", true);
            float target = Random.Range(rainLightRange.x, rainLightRange.y);
            if (globalLight != null)
                StartCoroutine(TransitionLight(globalLight.intensity, target));
        }
        else
        {
            AudioManager.Instance.StopSFX();
            if (globalLight != null)
                StartCoroutine(TransitionLight(globalLight.intensity, defaultLightIntensity));
        }
    }

    private IEnumerator TransitionLight(float from, float to)
    {
        float time = 0f;
        while (time < lightTransitionDuration)
        {
            time += Time.deltaTime;
            float t = time / lightTransitionDuration;
            globalLight.intensity = Mathf.Lerp(from, to, t);
            yield return null;
        }
        globalLight.intensity = to;
    }

    [ContextMenu("Test Rain")]
    public void ActiveRain() => SetWeather(true);

    public bool IsRaining() => isRaining;
}
