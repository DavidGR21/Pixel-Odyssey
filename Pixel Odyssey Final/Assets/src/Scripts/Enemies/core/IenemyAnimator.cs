/// <summary>
/// Interfaz encargada de manejar las animaciones de los enemigos.
/// Define los métodos necesarios para reproducir las animaciones de caminar, correr, atacar, recibir daño y morir.
/// Esta interfaz debe ser implementada por cualquier clase que maneje las animaciones de un enemigo.
/// </summary>
public interface IEnemyAnimator
{
    void PlayWalk(bool value);
    void PlayRun(bool value);
    void PlayAttack(bool value);
    void PlayHurt(bool value);
    void PlayDie();
    bool IsHurt();
}