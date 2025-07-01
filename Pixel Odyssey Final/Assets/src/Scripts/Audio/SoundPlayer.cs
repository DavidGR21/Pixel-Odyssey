using UnityEngine;
/// <summary>
/// Clase encargada de gestionar la reproducción de sonidos del jugador.
/// Implementa la interfaz IPlayerAudio para permitir la reproducción, detención y ajuste de volumen de los sonidos.
/// Esta clase debe ser añadida a un GameObject con un componente AudioSource para funcionar correctamente.
/// </summary>
public class SoundPlayer : IPlayerAudio
{
    private AudioSource audioSource;

    public SoundPlayer(AudioSource source)
    {
        audioSource = source;
    }

    public void PlaySound(AudioClip clip, bool preventOverlap = false)
    {
        if (clip == null) return;

        if (preventOverlap && audioSource.isPlaying && audioSource.clip == clip)
            return;

        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void StopSound(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource.isPlaying && audioSource.clip == clip)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public bool IsPlaying(AudioClip clip)
    {
        return audioSource.isPlaying && audioSource.clip == clip;
    }
}
