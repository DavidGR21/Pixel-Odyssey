/// <summary>
/// Interfaz que define el comportamiento de salud de un objeto en el juego.
/// Permite recibir daño, aplicar retroceso y verificar si el objeto está vivo.
/// Esta interfaz debe ser implementada por cualquier clase que maneje la salud de un objeto, como enemigos o el jugador.
/// </summary>
public interface IHealth
{
    void TakeDamage(float amount, UnityEngine.Vector2 knockbackDirection, float knockbackForce);
    bool IsAlive { get; }
}