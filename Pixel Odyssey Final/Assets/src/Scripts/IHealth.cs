public interface IHealth
{
    void TakeDamage(float amount, UnityEngine.Vector2 knockbackDirection, float knockbackForce);
    bool IsAlive { get; }
}