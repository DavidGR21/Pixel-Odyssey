using UnityEngine;
using System;
using System.Collections; // Added to include IEnumerator

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

    private Animator animator;
    private atackPlayer attackScript;
    private Rigidbody2D rb; // Cacheamos el Rigidbody2D para mejor rendimiento

    private void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        animator = GetComponent<Animator>();
        attackScript = GetComponent<atackPlayer>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"[HealthPlayer {gameObject.name}] No Rigidbody2D component found");
        }
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        if (currentHealth <= 0) return;

        bool damageApplied = false;

        // Primero el escudo absorbe daño
        if (currentShield > 0)
        {
            float shieldDamage = Mathf.Min(damage, currentShield);
            currentShield -= shieldDamage;
            damage -= shieldDamage;
            OnShieldChanged?.Invoke(currentShield); // Notifica cambio de escudo
            damageApplied = true;
        }

        // El resto del daño afecta la vida
        if (damage > 0)
        {
            currentHealth -= damage;
            OnHealthChanged?.Invoke(currentHealth); // Notifica cambio de salud
            damageApplied = true;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // Aplicar animación de daño y knockback si hubo daño real
        if (damageApplied)
        {
            animator.SetTrigger("Hurt");

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                Debug.Log($"[HealthPlayer {gameObject.name}] Knockback applied: direction={knockbackDirection}, force={knockbackForce}");
                StartCoroutine(StopMovementAfterKnockback());
            }
        }
    }

    private IEnumerator StopMovementAfterKnockback()
    {
        yield return new WaitForSeconds(0.2f); // Ajusta este tiempo si es necesario
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            Debug.Log($"[HealthPlayer {gameObject.name}] Movement stopped after knockback");
        }
    }

    private void Die() // metodo para manejar la muerte del jugador
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