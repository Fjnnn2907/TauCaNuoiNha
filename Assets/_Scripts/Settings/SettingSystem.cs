using UnityEngine;
using UnityEngine.UI;

public class SettingSystem : MonoBehaviour
{
    [Header("FPS")]
    public Button fps30Button;
    public Button fps60Button;
    public GameObject tick30;
    public GameObject tick60;

    private void Start()
    {
        fps30Button.onClick.AddListener(() => SetFPS(30));
        fps60Button.onClick.AddListener(() => SetFPS(60));

        int savedFPS = PlayerPrefs.GetInt("TargetFPS", 60);
        ApplyFPSUI(savedFPS);
        Application.targetFrameRate = savedFPS;

        // Tắt VSync để đảm bảo giới hạn FPS hoạt động
        QualitySettings.vSyncCount = 0;
    }

    void SetFPS(int fps)
    {
        Application.targetFrameRate = fps;
        PlayerPrefs.SetInt("TargetFPS", fps);
        ApplyFPSUI(fps);
    }

    void ApplyFPSUI(int fps)
    {
        bool is30 = fps == 30;

        tick30.SetActive(is30);
        tick60.SetActive(!is30);
    }
}
