/// <summary>
/// Interfaz que define el comportamiento de un enemigo con escudo.
/// Permite activar y desactivar el escudo, así como manejar el daño recibido cuando el escudo está activo.
/// </summary>
public interface IShieldEnemy
{
    bool IsShieldActive { get; }
    void ActivateShield();
    void DeactivateShield();
    bool TakeShieldedDamage(float damage, out float adjustedDamage);
}