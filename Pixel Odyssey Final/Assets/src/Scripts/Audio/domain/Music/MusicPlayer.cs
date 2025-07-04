using UnityEngine;
/// <summary>
/// Clase encargada de gestionar la reproducción de música en el juego.
/// Permite reproducir, detener y ajustar el volumen de la música de fondo.
/// </summary>
public class MusicPlayer : IPlayerAudio
{
    private AudioSource audioSource;

    public MusicPlayer(AudioSource source)
    {
        audioSource = source;
    }

    public void PlayMusic(AudioClip clip, bool loop)
    {
        if (clip == null) return;

        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void ToggleMute()
    {
        audioSource.mute = !audioSource.mute;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public void PlaySound(AudioClip clip, bool preventOverlap = false)
    {
        PlayMusic(clip, loop: true);
    }

    public void StopSound(AudioClip clip)
    {
        if (audioSource.clip == clip)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    public bool IsPlaying(AudioClip clip)
    {
        return audioSource.isPlaying && audioSource.clip == clip;
    }
}
