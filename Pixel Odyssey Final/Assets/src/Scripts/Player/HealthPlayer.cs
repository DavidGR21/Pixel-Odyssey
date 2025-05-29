using UnityEngine;
using System;

public class HealthPlayer : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Escudo")]
    [SerializeField] public float maxShield = 50f;
    [SerializeField] private float currentShield;

    public event Action OnDeath;  // Evento de muerte
    public event Action<float> OnHealthChanged; // Evento de cambio de vida
    public event Action<float> OnShieldChanged; // Evento de cambio de escudo

    [SerializeField] private float knockbackForce = 5f;
    private Animator animator;
    private atackPlayer attackScript;

    private void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        animator = GetComponent<Animator>();
        attackScript = GetComponent<atackPlayer>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        if (currentHealth <= 0) return;

        // Primero el escudo absorbe daño
        if (currentShield > 0)
        {
            float shieldDamage = Mathf.Min(damage, currentShield);
            currentShield -= shieldDamage;
            damage -= shieldDamage;
            OnShieldChanged?.Invoke(currentShield); // Notifica cambio de escudo
            animator.SetTrigger("Hurt");

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            }
        }

        // El resto del daño afecta la vida
        if (damage > 0)
        {
            currentHealth -= damage;
            OnHealthChanged?.Invoke(currentHealth); // Notifica cambio de salud

            animator.SetTrigger("Hurt");

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        OnDeath?.Invoke();

        if (attackScript != null)
            attackScript.canMove = false;

        GetComponent<MovementPlayer>().enabled = false;
        GetComponent<atackPlayer>().enabled = false;

        Destroy(gameObject, 3f);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth); 
    }

    public void HealShield(float amount)
    {
        currentShield = Mathf.Min(currentShield + amount, maxShield);
        OnShieldChanged?.Invoke(currentShield);
    }

}
