using UnityEngine;

public class HitEnemigo2D : MonoBehaviour
{
    private IMeleeEnemy meleeEnemy;

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
                Vector2 knockbackDirection = coll.transform.position - transform.position;
                playerHealth.TakeDamage(meleeEnemy.Damage, knockbackDirection);
                Debug.Log($"Enemigo infligió {meleeEnemy.Damage} de daño al jugador.");
            }
        }
        else if (coll.CompareTag("Player"))
        {
            Debug.Log($"[HitEnemigo2D {gameObject.name}] Colisión con el jugador, pero el enemigo no está atacando (IsAttacking={meleeEnemy?.IsAttacking}).");
        }
    }
}