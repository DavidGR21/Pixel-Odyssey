using UnityEngine;
/// <summary>
/// Interfaz que define el comportamiento de audio del jugador.
/// Permite reproducir sonidos, detenerlos, ajustar el volumen y verificar si un sonido está reproduciéndose.
/// Esta interfaz debe ser implementada por cualquier clase que maneje el audio del jugador.
/// </summary>
public interface IPlayerAudio
{
    void PlaySound(AudioClip clip, bool preventOverlap = false);
    void StopSound(AudioClip clip);
    void SetVolume(float volume);
    bool IsPlaying(AudioClip clip);
}
