using UnityEngine;

public class HitEnemigo2D : MonoBehaviour
{
    private IMeleeEnemy meleeEnemy;
    [SerializeField] private float knockbackForce = 5f; // Ajusta este valor para más o menos empuje

    private void Start()
    {
        meleeEnemy = GetComponentInParent<IMeleeEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player") && meleeEnemy != null && meleeEnemy.IsAttacking)
        {
            IHealth playerHealth = coll.GetComponent<IHealth>();
            if (playerHealth != null)
            {
                // Calcular la dirección del knockback como un vector que empuja al jugador lejos del enemigo
                Vector2 knockbackDirection = (coll.transform.position - transform.position).normalized;
                // Invertir la dirección para empujar al jugador lejos del enemigo
                knockbackDirection = -knockbackDirection;
                playerHealth.TakeDamage(meleeEnemy.Damage, knockbackDirection, knockbackForce);
            }
        }
    }
}