using UnityEngine;

public interface IPlayerAudio
{
    void PlaySound(AudioClip clip, bool preventOverlap = false);
    void StopSound(AudioClip clip);
    void SetVolume(float volume);
    bool IsPlaying(AudioClip clip);
}
