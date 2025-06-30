using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AudioSources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource playerSFXAudioSource;
    [SerializeField] private AudioSource uiAudioSource;

    private IPlayerAudio musicPlayer;
    private IPlayerAudio playerSFXPlayer;
    //private IPlayerAudio enemySFXPlayer;
    //private IPlayerAudio ambiencePlayer;
    private IPlayerAudio uiPlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicPlayer = new MusicPlayer(musicAudioSource);
        playerSFXPlayer = new SoundPlayer(playerSFXAudioSource);
        uiPlayer = new SoundPlayer(uiAudioSource);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMusicMute();
        }
    }

    // Música
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        ((MusicPlayer)musicPlayer).PlayMusic(clip, loop);
    }

    public void ToggleMusicMute()
    {
        ((MusicPlayer)musicPlayer).ToggleMute();
    }

    public void SetMusicVolume(float volume)
    {
        musicPlayer.SetVolume(volume);
    }

    // SFX Jugador
    public void PlayPlayerSound(AudioClip clip, bool preventOverlap = false)
    {
        playerSFXPlayer.PlaySound(clip, preventOverlap);
    }

    public void StopPlayerSound(AudioClip clip)
    {
        playerSFXPlayer.StopSound(clip);
    }

    public void SetPlayerSFXVolume(float volume)
    {
        playerSFXPlayer.SetVolume(volume);
    }

    //UI Sounds

    public void PlayUISound(AudioClip clip, bool preventOverlap = false)
    {
        uiPlayer.PlaySound(clip, preventOverlap);
    }

}
