using UnityEngine;

public interface IMeleeEnemy 
{
    void Attack(); // Inicia el ataque melee
    void StopAttack(); // Detiene el ataque melee
    bool IsAttacking { get; } // Estado del ataque
    float AttackRange { get; } // Rango de ataque
    int Damage { get; } // Daño del ataque
    void EnableAttackCollider(bool enable); // Controla el colisionador de ataque
    
}
