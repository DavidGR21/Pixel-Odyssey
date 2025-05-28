using UnityEngine;

public class HealthPlayer : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float knockbackForce = 5f;
    private Animator animator;
    private atackPlayer attackScript;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        attackScript = GetComponent<atackPlayer>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        // Aplicar retroceso
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Opcional: para reiniciar velocidad actual
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        animator.SetTrigger("Die");

        if (attackScript != null)
            attackScript.canMove = false;

        // Opcional: desactiva todos los scripts que controlan al jugador
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        // También puedes desactivar scripts específicos si los tienes
        if (TryGetComponent<MovementPlayer>(out var move)) move.enabled = false;
        if (TryGetComponent<atackPlayer>(out var atk)) atk.enabled = false;
    }

}
