using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Clase base para todos los enemigos del juego.
/// </summary>
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
    public int direction;
    public int rutine;
    public float chronometer;
    public GameObject target;

    // campos para detección de suelo y paredes
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.1f;

    protected IEnemyAnimator enemyAnimator;

    // Se inyecta el animador del enemigo
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
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
        }
    }
    // Método para establecer el comportamiento del enemigo
    /// <summary>
    /// Establece el comportamiento actual del enemigo.
    /// </summary>
    /// <param name="behavior">El comportamiento a establecer.</param>
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
            return;
        }
        currentBehavior?.Execute(this);
    }

    /// <summary>
    /// Aplica daño al enemigo y maneja la lógica de retroceso.
    /// </summary>      
    public virtual void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        // Verificar si el enemigo tiene escudo y bloquear daño si aplica
        float adjustedDamage = damage;
        if (this is IShieldEnemy shieldEnemy)
        {
            if (shieldEnemy.TakeShieldedDamage(damage, out adjustedDamage))
            {
                return; // Salir si el daño está bloqueado
            }
        }

        // Aplicar daño ajustado
        currentHealth -= adjustedDamage;
        Animator animator = GetComponent<Animator>();
        if (animator != null && !isHurtActive && adjustedDamage > 0) // Activar hurt solo si hay daño real
        {
            isHurtActive = true;
            animator.SetBool("hurt", true);
            StartCoroutine(ResetHurtAnimation(animator));
        }


        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(StopMovementAfterKnockback());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    /// <summary>
    /// Detiene el movimiento del enemigo después de recibir un retroceso.
    /// </summary>
    private IEnumerator StopMovementAfterKnockback()
    {
        yield return new WaitForSeconds(0.2f);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    /// <summary>
    /// Obtiene la duración de la animación de daño.
    /// </summary>
    private float GetHurtAnimationDuration(Animator animator)
    {
        if (animator == null)
        {
            return 0.3f; // Valor por defecto si no se puede obtener
        }

        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
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
                        return duration;
                    }
                    else
                    {
                        return 0.3f;
                    }
                }
            }
        }

        return 0.3f;
    }

    /// <summary>
    /// Reinicia la animación de daño después de un tiempo.
    /// </summary>
    private IEnumerator ResetHurtAnimation(Animator animator)
    {
        float hurtAnimationDuration = GetHurtAnimationDuration(animator);
        yield return new WaitForSeconds(hurtAnimationDuration);
        if (animator != null && currentHealth > 0)
        {
            animator.SetBool("hurt", false);
            isHurtActive = false;
        }
    }
    /// <summary>
    /// Maneja la muerte del enemigo.
    /// Desactiva el collider, cambia el Rigidbody a estático y reproduce la animación de muerte.
    /// Si es un jefe, carga la escena de créditos después de un tiempo.
    /// </summary>
    protected virtual void Die()
    {
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

/// <summary>
/// Metodo para destruir el objeto después de la animación de muerte.
/// </summary>
    private IEnumerator DestroyAfterDeathAnimation()
    {
        float deathAnimationDuration = 1.2f;
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
}