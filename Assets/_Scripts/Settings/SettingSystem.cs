using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingSystem : MonoBehaviour
{
    [Header("FPS")]
    public Button fps30Button;
    public Button fps60Button;
    public GameObject tick30;
    public GameObject tick60;

    [Header("Resolution")]
    public TMP_Dropdown resolutionDropdown;

    private readonly Vector2Int[] predefinedResolutions = new Vector2Int[]
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1280, 720),
        //new Vector2Int(800, 600)
    };

    private void Start()
    {
        // --- FPS ---
        fps30Button.onClick.AddListener(() => SetFPS(30));
        fps60Button.onClick.AddListener(() => SetFPS(60));
        int savedFPS = PlayerPrefs.GetInt("TargetFPS", 60);
        SetFPS(savedFPS);
        QualitySettings.vSyncCount = 0;

        // --- Resolution Dropdown ---
        SetupResolutionDropdown();

        // Load saved resolution
        int savedWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int savedHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        SetResolution(savedWidth, savedHeight);
    }

    // ------------------ FPS ------------------

    void SetFPS(int fps)
    {
        Application.targetFrameRate = fps;
        PlayerPrefs.SetInt("TargetFPS", fps);
        ApplyFPSUI(fps);
    }

    void ApplyFPSUI(int fps)
    {
        tick30.SetActive(fps == 30);
        tick60.SetActive(fps == 60);
    }

    // ------------------ Resolution ------------------

    void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();

        int defaultIndex = 0;
        int savedWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int savedHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);

        for (int i = 0; i < predefinedResolutions.Length; i++)
        {
            Vector2Int res = predefinedResolutions[i];
            string option = $"{res.x} x {res.y}";
            options.Add(option);

            if (res.x == savedWidth && res.y == savedHeight)
                defaultIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = defaultIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
    }

    void OnResolutionDropdownChanged(int index)
    {
        Vector2Int selectedRes = predefinedResolutions[index];
        SetResolution(selectedRes.x, selectedRes.y);
    }

    void SetResolution(int width, int height)
    {
        bool isFullHD = (width == 1920 && height == 1080);
        FullScreenMode mode = isFullHD ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;

        Screen.SetResolution(width, height, mode);

        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        PlayerPrefs.SetInt("IsFullscreen", isFullHD ? 1 : 0);
    }
}
