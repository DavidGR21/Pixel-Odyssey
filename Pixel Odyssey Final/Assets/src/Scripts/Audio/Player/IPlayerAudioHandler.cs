/// <summary>
/// Interfaz encargada de manejar los sonidos del jugador.
/// Define los métodos necesarios para reproducir sonidos de pasos, salto, daño, ataque y dash.
/// Esta interfaz debe ser implementada por cualquier clase que maneje los sonidos del jugador.
/// Permite la reproducción y detención de sonidos específicos relacionados con las acciones del jugador.
/// </summary>
public interface IPlayerAudioHandler
{
    void PlayStepSound(bool loop);
    void StopStepSound();
    void PlayJumpSound();
    void PlayDamageSound();
    void PlayAttackSound();
    void PlayDashSound();
}
