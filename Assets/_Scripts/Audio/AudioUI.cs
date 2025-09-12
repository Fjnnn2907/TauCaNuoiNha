using UnityEngine;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button nextMusicButton;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        nextMusicButton?.onClick.AddListener(OnNextMusicClicked);
        musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider?.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnNextMusicClicked()
    {
        AudioManager.Instance?.PlayNextTrack();
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }
    public void RefreshSliders()
    {
        if (musicVolumeSlider == null || musicVolumeSlider == null)
            return;

        musicVolumeSlider?.SetValueWithoutNotify(AudioManager.Instance.musicVolume);
        sfxVolumeSlider?.SetValueWithoutNotify(AudioManager.Instance.sfxVolume);
    }
}
