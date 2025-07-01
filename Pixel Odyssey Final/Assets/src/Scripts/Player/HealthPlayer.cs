using UnityEngine;
using System;
using System.Collections; // Added to include IEnumerator
/// <summary>
/// Clase encargada de gestionar la salud y el escudo del jugador.
/// Implementa la interfaz IHealth para definir el comportamiento de salud del jugador.
/// Permite al jugador recibir daño, curarse, restaurar escudo y manejar eventos de muerte y cambio de salud.
/// </summary>
public class HealthPlayer : MonoBehaviour, IHealth

// Este patrón permite que otros objetos se suscriban a cambios en el estado del jugador (vida, escudo, muerte).
// Así, por ejemplo, una barra de vida en la UI puede actualizarse automáticamente cuando cambia currentHealth.
{
    [Header("Vida")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float currentHealth;
    public float CurrentHealth => currentHealth;
    public float CurrentShield => currentShield;
    [Header("Escudo")]
    [SerializeField] public float maxShield = 50f;
    [SerializeField] public float currentShield;

    public event Action OnDeath;  // Evento de muerte
    public event Action<float> OnHealthChanged; // Evento de cambio de vida
    public event Action<float> OnShieldChanged; // Evento de cambio de escudo

    private Animator animator;
    private atackPlayer attackScript;
    private Rigidbody2D rb; // Cacheamos el Rigidbody2D para mejor rendimiento

    public bool IsAlive => currentHealth > 0;

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
    // En HealthPlayer.cs
    public void RestoreHealthAndShield(float health, float shield)
    {
        currentHealth = health;
        currentShield = shield;
        OnHealthChanged?.Invoke(currentHealth);
        OnShieldChanged?.Invoke(currentShield);
    }
    public void TakeDamage(float amount, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        if (currentHealth <= 0) return;

        bool damageApplied = false;
        float damage = amount;

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
            if (animator != null)
                animator.SetTrigger("Hurt");
            var movement = GetComponent<MovementPlayer>();
            movement?.audioHandler?.PlayDamageSound();

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
        if (animator != null)
            animator.SetTrigger("Die");
        OnDeath?.Invoke();

        if (attackScript != null)
            attackScript.canMove = false;

        var movement = GetComponent<MovementPlayer>();
        if (movement != null)
            movement.enabled = false;

        var attack = GetComponent<atackPlayer>();
        if (attack != null)
            attack.enabled = false;


        // Evitar movimiento horizontal tras la muerte
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Detiene cualquier movimiento actual
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        //  if (PauseManager.Instance != null)
        //    PauseManager.Instance.ShowGameOverMenu();
        //   Destroy(gameObject, 3f);
        StartCoroutine(ShowGameOverMenuDelayed());

    }

    private IEnumerator ShowGameOverMenuDelayed()
    {
        yield return new WaitForSeconds(1f);
        if (PauseManager.Instance != null)
            PauseManager.Instance.ShowGameOverMenu();
    }
    public void Revive(float health, float shield)
    {
        // Restaurar salud y escudo
        RestoreHealthAndShield(health, shield);

        // Reactivar componentes de movimiento y ataque
        var movement = GetComponent<MovementPlayer>();
        if (movement != null)
            movement.enabled = true;

        var attack = GetComponent<atackPlayer>();
        if (attack != null)
            attack.enabled = true;

        if (attackScript != null)
            attackScript.canMove = true;

        // Quitar restricciones del Rigidbody2D
        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Resetear animación a estado idle o similar
        if (animator != null)
        {
            animator.Play("PlayerIdle");
            Debug.Log("Animacion idle");
        }   
        // Si tienes otros estados a restaurar, hazlo aquí
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