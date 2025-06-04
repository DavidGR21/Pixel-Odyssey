using UnityEngine;

public interface IMeleeEnemy 
{
    void Attack(); // Inicia el ataque melee
    void StopAttack(); // Detiene el ataque melee
    bool IsAttacking { get; } // Estado del ataque
    float AttackRange { get; } 
    int Damage { get; } 
    void EnableAttackCollider(bool enable); 
}
