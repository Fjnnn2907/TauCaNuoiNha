using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>, ISaveable
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioUI audioUI;

    [Header("Volume")]
    [Range(0, 1)] public float musicVolume = 1;
    [Range(0, 1)] public float sfxVolume = 1;

    [Header("Music Playlist")]
    public List<string> musicTracks = new(); // Danh sach ten file trong Resources/Audio/Music

    private int currentTrackIndex = -1;

    private void Start()
    {
        SaveManager.Instance?.RegisterSaveable(this);
        PlayRandomMusic();
    }
    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance?.UnregisterSaveable(this);
    }
    private void Update()
    {
        if (!musicSource.isPlaying)
            PlayNextTrack();
    }

    /// <summary>
    /// Phát âm thanh ngắn
    /// </summary>
    public void PlaySFX(string sfxName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/" + sfxName);
        if (clip)
            sfxSource.PlayOneShot(clip, sfxVolume);
        else
            Debug.LogWarning("SFX không tìm thấy: " + sfxName);
    }

    /// <summary>
    /// Phát nhạc nền theo tên
    /// </summary>
    public void PlayMusic(string musicName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/Music/" + musicName);
        if (clip && musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.loop = false; // Không lặp để còn chuyển bài
            musicSource.Play();
        }
    }

    /// <summary>
    /// Phát nhạc ngẫu nhiên khi bắt đầu
    /// </summary>
    private void PlayRandomMusic()
    {
        if (musicTracks.Count == 0) return;

        currentTrackIndex = Random.Range(0, musicTracks.Count);
        PlayMusic(musicTracks[currentTrackIndex]);
    }

    /// <summary>
    /// Chuyển sang bài tiếp theo trong danh sách
    /// </summary>
    public void PlayNextTrack()
    {
        if (musicTracks.Count == 0) return;

        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Count;
        PlayMusic(musicTracks[currentTrackIndex]);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
    }

    #region ISaveable
    public void SaveData(ref GameData data)
    {
        data.savedMusicVolume = musicVolume;
        data.savedSFXVolume = sfxVolume;
    }

    public void LoadData(GameData data)
    {
        musicVolume = data.savedMusicVolume;
        sfxVolume = data.savedSFXVolume;

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        audioUI.RefreshSliders();
    }
    #endregion
}
