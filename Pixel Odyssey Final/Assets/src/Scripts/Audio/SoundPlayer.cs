using UnityEngine;

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
