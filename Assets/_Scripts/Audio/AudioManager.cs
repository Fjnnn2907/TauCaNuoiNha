using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioUI audioUI;

    [Header("Volume")]
    [Range(0, 1)] public float musicVolume = 1;
    [Range(0, 1)] public float sfxVolume = 1;

    [Header("Music Playlist")]
    public List<string> musicTracks = new(); // Danh sách tên file trong Resources/Audio/Music

    private int currentTrackIndex = -1;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Start()
    {
        LoadSettings();
        PlayRandomMusic();
    }

    private void Update()
    {
        if (!musicSource.isPlaying)
            PlayNextTrack();
    }

    public void PlaySFX(string sfxName, bool isLoop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/" + sfxName);
        if (clip)
        {
            if (isLoop)
            {
                // Dùng sfxSource để loop
                sfxSource.clip = clip;
                sfxSource.loop = true;
                sfxSource.volume = sfxVolume;
                sfxSource.Play();
            }
            else
                sfxSource.PlayOneShot(clip, sfxVolume);
        }
        else
            Debug.LogWarning("SFX không tìm thấy: " + sfxName);
    }

    public void PlayMusic(string musicName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/Music/" + musicName);
        if (clip && musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.loop = false;
            musicSource.Play();
        }
    }

    private void PlayRandomMusic()
    {
        if (musicTracks.Count == 0) return;

        currentTrackIndex = Random.Range(0, musicTracks.Count);
        PlayMusic(musicTracks[currentTrackIndex]);
    }

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
    public void StopSFX()
    {
        sfxSource.Stop();
        sfxSource.clip = null;
        sfxSource.loop = false;
    }
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = musicVolume;
        SaveSettings();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = sfxVolume;
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        audioUI?.RefreshSliders();
    }
}
