using UnityEngine;

public class HealthPlayer : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    private atackPlayer attackScript;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        attackScript = GetComponent<atackPlayer>();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

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
    }
}
