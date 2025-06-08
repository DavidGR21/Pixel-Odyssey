using UnityEngine;

public class HitEnemigo2D : MonoBehaviour
{
    private IMeleeEnemy meleeEnemy;
    [SerializeField] private float knockbackForce = 5f; // Ajusta este valor para más o menos empuje

    private void Start()
    {
        meleeEnemy = GetComponentInParent<IMeleeEnemy>();
        if (meleeEnemy == null)
        {
            Debug.LogError($"No se encontró IMeleeEnemy en el padre de {gameObject.name}.");
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player") && meleeEnemy != null && meleeEnemy.IsAttacking)
        {
            HealthPlayer playerHealth = coll.GetComponent<HealthPlayer>();
            if (playerHealth != null)
            {
                // Calcular la dirección del knockback como un vector que empuja al jugador lejos del enemigo
                Vector2 knockbackDirection = (coll.transform.position - transform.position).normalized;
                // Invertir la dirección para empujar al jugador lejos del enemigo
                knockbackDirection = -knockbackDirection;
                playerHealth.TakeDamage(meleeEnemy.Damage, knockbackDirection, knockbackForce);
                Debug.Log($"Enemigo infligió {meleeEnemy.Damage} de daño al jugador con knockback force={knockbackForce}, direction={knockbackDirection}.");
            }
        }
        else if (coll.CompareTag("Player"))
        {
            Debug.Log($"[HitEnemigo2D {gameObject.name}] Colisión con el jugador, pero el enemigo no está atacando (IsAttacking={meleeEnemy?.IsAttacking}).");
        }
    }
}