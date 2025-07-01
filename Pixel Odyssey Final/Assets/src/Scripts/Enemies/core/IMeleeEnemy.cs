using UnityEngine;
/// <summary>
/// Interfaz encargada de manejar el comportamiento de ataque melee de los enemigos.
/// Esta interfaz debe ser implementada por cualquier clase que implemente un enemigo con ataque melee.
/// </summary>
public interface IMeleeEnemy
{
    void Attack(); // Inicia el ataque melee
    void StopAttack(); // Detiene el ataque melee
    bool IsAttacking { get; } // Estado del ataque
    float AttackRange { get; }
    int Damage { get; }
    void EnableAttackCollider(bool enable);
}
