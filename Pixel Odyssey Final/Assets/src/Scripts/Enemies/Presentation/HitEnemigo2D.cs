using UnityEngine;

public class HitEnemigo2D : MonoBehaviour
{
    private IMeleeEnemy meleeEnemy;
    [SerializeField] private float knockbackForce = 5f;
    private bool hasDealtDamage = false;

    private void Start()
    {
        meleeEnemy = GetComponentInParent<IMeleeEnemy>();
    }

    private void OnEnable()
    {
        // Permite que se haga daño cada vez que se activa el collider para un nuevo ataque
        hasDealtDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        TryDealDamage(coll);
    }

    private void OnTriggerStay2D(Collider2D coll)
    {
        TryDealDamage(coll);
    }

    private void TryDealDamage(Collider2D coll)
    {
        if (coll.CompareTag("Player") && meleeEnemy != null && meleeEnemy.IsAttacking && !hasDealtDamage)
        {
            IHealth playerHealth = coll.GetComponent<IHealth>();
            if (playerHealth != null)
            {
                // Calcular la dirección del knockback como un vector que empuja al jugador lejos del enemigo
                Vector2 knockbackDirection = (coll.transform.position - transform.position).normalized;
                // Invertir la dirección para empujar al jugador lejos del enemigo
                knockbackDirection = -knockbackDirection;
                playerHealth.TakeDamage(meleeEnemy.Damage, knockbackDirection, knockbackForce);
                hasDealtDamage = true; // Solo permite un daño por ataque
            }
        }
    }

    public void ResetDamage()
    {
        hasDealtDamage = false;
    }
}