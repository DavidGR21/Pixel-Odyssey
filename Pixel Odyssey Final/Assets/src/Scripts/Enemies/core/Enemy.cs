using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;
    protected Rigidbody2D rb;
    protected bool isHurtActive = false;
    protected IEnemyBehavior currentBehavior;

    // Campos necesarios para los comportamientos
    public float speedWalk;
    public float speedRun;
    public float visionRange;
    public float attackRange;
    public int direccion;
    public int rutina;
    public float cronometro;
    public GameObject target;

    // Nuevos campos para detección de suelo y paredes
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.1f;

    protected IEnemyAnimator enemyAnimator;

    public virtual void InjectAnimator(IEnemyAnimator animator)
    {
        this.enemyAnimator = animator;
    }

    public abstract IEnemyAnimator GetAnimator();

    public virtual void Initialize()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning($"[Enemy {gameObject.name}] No Rigidbody2D component found");
        }
    }

    public void SetBehavior(IEnemyBehavior behavior)
    {
        currentBehavior = behavior;
    }
    protected virtual void Update()
    {
        UpdateBehavior();
    }

    public virtual void UpdateBehavior()
    {
        if (isHurtActive)
        {
            Debug.Log($"[Enemy {gameObject.name}] Está herido, no ejecuta behaviors.");
            return;
        }
        currentBehavior?.Execute(this);
    }

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
            return 0.3f; // Valor por defecto si no se puede obtener
        }

        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            Debug.LogError($"[Enemy {gameObject.name}] AnimatorController not found, using default duration");
            return 0.3f;
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
                        Debug.Log($"[Enemy {gameObject.name}] Hurt an  imation duration: {duration} seconds (clip length: {clip.length}, speed: {state.state.speed})");
                        return duration;
                    }
                    else
                    {
                        Debug.LogWarning($"[Enemy {gameObject.name}] No AnimationClip found for Hurt state, using default duration");
                        return 0.3f;
                    }
                }
            }
        }

        Debug.LogWarning($"[Enemy {gameObject.name}] Hurt state not found in AnimatorController, using default duration");
        return 0.3f;
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
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Si es jefe, ir a créditos
        if (this is BossEnemy)
        {
            Debug.Log("[Enemy] Boss muerto. Cargando créditos...");
            StartCoroutine(LoadCreditsScene());
        }
        else
        {
            StartCoroutine(DestroyAfterDeathAnimation());
        }
    }

    private IEnumerator LoadCreditsScene()
    {
        float deathAnimationDuration = 0.5f;
        yield return new WaitForSeconds(deathAnimationDuration);
        SceneManager.LoadScene("Credits");

        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("MainMenu");
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