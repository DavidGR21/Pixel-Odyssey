public interface IShieldEnemy
{
    bool IsShieldActive { get; }
    void ActivateShield();
    void DeactivateShield();
    bool TakeShieldedDamage(float damage, out float adjustedDamage);
}