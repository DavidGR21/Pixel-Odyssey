using UnityEngine;
using UnityEditor.Animations; // Necesario para trabajar con AnimatorController
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;
    protected Rigidbody2D rb;
    private bool isHurtActive = false;

    public virtual void Initialize()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning($"[Enemy {gameObject.name}] No Rigidbody2D component found");
        }
    }

    public abstract void UpdateBehavior();

    public virtual void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        Debug.Log($"[Enemy {gameObject.name}] TakeDamage called: damage={damage}, currentHealth={currentHealth} -> {currentHealth - damage} at frame {Time.frameCount}");

        // Verificar si el enemigo tiene escudo y bloquear daño si aplica
        float adjustedDamage = damage;
        if (this is IShieldEnemy shieldEnemy)
        {
            if (shieldEnemy.TakeShieldedDamage(damage, out adjustedDamage))
            {
                Debug.Log($"[Enemy {gameObject.name}] Damage blocked by shield: {damage} reduced to {adjustedDamage}");
                return; // Salir si el daño está bloqueado
            }
        }

        // Aplicar daño ajustado
        currentHealth -= adjustedDamage;
        Debug.Log($"[Enemy {gameObject.name}] Damage applied: new health={currentHealth}");

        Animator animator = GetComponent<Animator>();
        if (animator != null && !isHurtActive && adjustedDamage > 0) // Activar hurt solo si hay daño real
        {
            isHurtActive = true;
            animator.SetBool("hurt", true);
            Debug.Log($"[Enemy {gameObject.name}] Hurt animation triggered at frame {Time.frameCount}");
            StartCoroutine(ResetHurtAnimation(animator));
        }
        else if (isHurtActive)
        {
            Debug.LogWarning($"[Enemy {gameObject.name}] Hurt animation already active, skipping reactivation at frame {Time.frameCount}");
        }
        else if (animator == null)
        {
            Debug.LogError($"[Enemy {gameObject.name}] No Animator component found in TakeDamage()");
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(StopMovementAfterKnockback());
            Debug.Log($"[Enemy {gameObject.name}] Knockback applied: force={knockbackForce}, direction={knockbackDirection}");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator StopMovementAfterKnockback()
    {
        yield return new WaitForSeconds(0.2f);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            Debug.Log($"[Enemy {gameObject.name}] Movement stopped after knockback");
        }
    }

    private float GetHurtAnimationDuration(Animator animator)
    {
        if (animator == null)
        {
            Debug.LogError($"[Enemy {gameObject.name}] Animator is null, cannot get hurt animation duration");
            return 0.8f; // Valor por defecto si no se puede obtener
        }

        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            Debug.LogError($"[Enemy {gameObject.name}] AnimatorController not found, using default duration");
            return 0.8f;
        }

        foreach (var layer in animatorController.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.name == "hurt")
                {
                    AnimationClip clip = state.state.motion as AnimationClip;
                    if (clip != null)
                    {
                        float duration = clip.length / (state.state.speed != 0 ? state.state.speed : 1);
                        Debug.Log($"[Enemy {gameObject.name}] Hurt animation duration: {duration} seconds (clip length: {clip.length}, speed: {state.state.speed})");
                        return duration;
                    }
                    else
                    {
                        Debug.LogWarning($"[Enemy {gameObject.name}] No AnimationClip found for Hurt state, using default duration");
                        return 0.8f;
                    }
                }
            }
        }

        Debug.LogWarning($"[Enemy {gameObject.name}] Hurt state not found in AnimatorController, using default duration");
        return 0.8f;
    }

    private IEnumerator ResetHurtAnimation(Animator animator)
    {
        float hurtAnimationDuration = GetHurtAnimationDuration(animator);
        Debug.Log($"[Enemy {gameObject.name}] Waiting to reset hurt animation ({hurtAnimationDuration} seconds)");
        yield return new WaitForSeconds(hurtAnimationDuration);
        if (animator != null && currentHealth > 0)
        {
            animator.SetBool("hurt", false);
            isHurtActive = false;
            Debug.Log($"[Enemy {gameObject.name}] Hurt animation completed, hurt set to false at frame {Time.frameCount}");
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"[Enemy {gameObject.name}] Die called, triggering Dead animation");
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("dead");
            Debug.Log($"[Enemy {gameObject.name}] Dead trigger set, Animator enabled: {animator.enabled}");
        }
        else
        {
            Debug.LogError($"[Enemy {gameObject.name}] No Animator component found in Die()");
            return;
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            Debug.Log($"[Enemy {gameObject.name}] Collider disabled");
        }
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
            Debug.Log($"[Enemy {gameObject.name}] Rigidbody set to Static");
        }

        StartCoroutine(DestroyAfterDeathAnimation());
    }

    private IEnumerator DestroyAfterDeathAnimation()
    {
        float deathAnimationDuration = 1.2f;
        Debug.Log($"[Enemy {gameObject.name}] Waiting for death animation to complete ({deathAnimationDuration} seconds)");
        yield return new WaitForSeconds(deathAnimationDuration);
        Debug.Log($"[Enemy {gameObject.name}] Death animation completed, destroying GameObject");
        Destroy(gameObject);
    }
}