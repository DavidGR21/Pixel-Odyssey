/// <summary>
/// Interfaz encargada de manejar las animaciones de enemigos con escudo.
/// Define los métodos necesarios para reproducir animaciones de escudo, además de las animaciones básicas de enemigos.
/// Esta interfaz debe ser implementada por cualquier clase que maneje las animaciones de un enemigo con escudo.
/// </summary>
public interface IShieldEnemyAnimator : IEnemyAnimator
{
    void PlayShield(bool value);
}