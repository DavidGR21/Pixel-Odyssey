using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// Clase que representa al jefe enemigo en el juego.
/// Hereda de la clase Enemy y maneja la transformación del jefe, su comportamiento de combate,
/// ataques especiales y la interacción con el jugador.
/// </summary>
public class BossEnemy : Enemy
{
    [Header("Transformación del Jefe")]
    [SerializeField] private Animator animator;
    [SerializeField] private float transformDuration = 3.2f;
    private MovementPlayer playerScript;
    GameObject playerObj;
    public bool hasTransformed = false;
    public bool isTransforming = false;
    private bool firstHitReceived = false;

    [Header("Tamaño del Collider")]
    private BoxCollider2D boxCollider;
    [SerializeField] private Vector2 originalColliderSize;
    [SerializeField] private Vector2 transformedColliderSize;
    [SerializeField] private Vector2 originalColliderOffset;
    [SerializeField] private Vector2 transformedColliderOffset;

    [Header("Movimiento del Jefe")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float stopDistance = 1f;
    public Transform playerTransform;

    [Header("Atacke Melee del Jefe")]
    [SerializeField] private BossAttackBase[] bossAttacks;
    [SerializeField] private BossAttackMelee meleeAttack;

    [Header("Atacke Flame del Jefe")]
    [SerializeField] private BossAttackFlamethrower flameAttack;

    [Header("Atacke Jump del Jefe")]
    [SerializeField] private BossAttackJump jumpAttack;

    [Header("Musica Batalla Final del Juego")]
    [SerializeField] private AudioClip sceneMusic;

    public void TriggerEnableHitbox() => meleeAttack.EnableHitbox();
    public void TriggerDisableHitbox() => meleeAttack.DisableHitbox();

    public void TriggerEnableHitboxFlame() => flameAttack.EnableHitbox();
    public void TriggerDisableHitboxFlame() => flameAttack.DisableHitbox();
    public void TriggerEnableHitboxJump() => jumpAttack.EnableHitbox();
    public void TriggerDisableHitboxJump() => jumpAttack.DisableHitbox();
    public  IEnemyAnimator GetAnimator()
    {
        return null;
    }
    private void Start()
    {
        health.CurrentHealth = 200f; // No tiene salud hasta transformarse

        if (animator == null)
            animator = GetComponent<Animator>();

        playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerScript = playerObj.GetComponent<MovementPlayer>();
            playerTransform = playerObj.transform;
        }

        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            // Si no quieres asignar por Inspector, lo puedes capturar aquí:
            originalColliderSize = boxCollider.size;
            originalColliderOffset = boxCollider.offset;
        }
    }

    private void Update()
    {
        UpdateBehavior();

        // Verificar ataques
        foreach (var attack in bossAttacks)
        {
            if (attack.CanAttack())
            {
                attack.ExecuteAttack();
                break; // solo un ataque a la vez
            }
        }
    }


    public  void UpdateBehavior()
    {
        if (!hasTransformed || isTransforming) return;
        if (playerTransform == null) return;

        float distanceX = Mathf.Abs(playerTransform.position.x - transform.position.x);

        // Si está lejos, moverse horizontalmente hacia el jugador
        if (distanceX > stopDistance)
        {
            Vector2 targetPosition = new Vector2(playerTransform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Girar al jugador
            Vector3 scale = transform.localScale;
            scale.x = playerTransform.position.x > transform.position.x ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;

            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
    private IEnumerator TransformBoss()
    {
        Debug.Log("[Boss] Recibió su primer golpe. Iniciando transformación...");

        isTransforming = true;

        AudioManager.Instance.PlayMusic(sceneMusic, true);

        // Bloquear jugador
        if (playerScript != null)
        {
            playerScript.BlockPlayerControl(true);

            float direction = Mathf.Sign(playerScript.transform.position.x - transform.position.x);
            Vector2 knockback = new Vector2(direction, 0f).normalized * 70f; // Solo eje X, fuerza ajustable

            playerScript.rb2d.linearVelocity = Vector2.zero; // Reset previo
            playerScript.rb2d.AddForce(knockback, ForceMode2D.Impulse);
            if (playerScript.animator != null)
            {
                animator.SetTrigger("transform"); // Trigger de animación
                playerScript.animator.SetTrigger("Hurt");
                yield return new WaitForSeconds(0.3f);
                playerScript.animator.Play("PlayerIdle");
            }
        }

        yield return new WaitForSeconds(transformDuration);

        health.CurrentHealth = maxHealth;
        hasTransformed = true;
        if (boxCollider != null)
        {
            boxCollider.size = transformedColliderSize;
            boxCollider.offset = transformedColliderOffset;

            Debug.Log("[Boss] Collider cambiado tras transformación.");
        }
        isTransforming = false;

        // Restaurar movimiento del jugador
        if (playerScript != null)
        {
            playerScript.BlockPlayerControl(false);
        }

        Debug.Log("[Boss] Transformación completada. Fase de combate activa.");
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 200f)
    {
        if (!firstHitReceived)
        {
            firstHitReceived = true;
            StartCoroutine(TransformBoss());
            return;
        }
        if (!hasTransformed || isTransforming) return;
        Vector2 horizontalKnockback = new Vector2(Mathf.Sign(knockbackDirection.x), 0f).normalized * knockbackForce;
        base.TakeDamage(damage, horizontalKnockback, knockbackForce);

    }

    public bool IsTransformed()
    {
        return hasTransformed && !isTransforming;
    }

    public void StopMovement()
    {
        animator.SetBool("isMoving", false); // Para detener animación
        moveSpeed = 0f;
    }

    public void ResumeMovement()
    {
        moveSpeed = 3.5f; // O el valor original que usas
    }

    public Transform PlayerTransform => playerTransform;


}
