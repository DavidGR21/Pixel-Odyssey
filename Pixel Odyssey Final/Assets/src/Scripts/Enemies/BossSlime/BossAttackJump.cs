using UnityEngine;
/// <summary>
/// Clase que representa el ataque de salto del jefe Slime.
/// </summary>
public class BossAttackJump : BossAttackBase
{
    [Header("Condiciones del Ataque")]
    [SerializeField] private float minDistanceToTrigger = 3.5f;
    [SerializeField] private float attackCooldown = 6f;

    [Header("Collider del Ataque")]
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private Transform hitboxPosition;
    [SerializeField] private float hitboxRadius = 1f;

    [Header("Parámetros del salto")]
    [SerializeField] private float horizontalForce = 6f;

    private float lastAttackTime = -999f;
    private Rigidbody2D rb;

    private void Awake()
    {
        boss = GetComponentInParent<BossEnemy>();
        rb = boss.GetComponent<Rigidbody2D>();
        attackHitbox.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, minDistanceToTrigger);

            if (hitboxPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(hitboxPosition.position, hitboxRadius);
            }
        }
    }

    public override bool CanAttack()
    {
        if (!boss.IsTransformed()) return false;

        float distance = Mathf.Abs(boss.PlayerTransform.position.x - transform.position.x);
        if (distance <= minDistanceToTrigger) return false;

        return Time.time >= lastAttackTime + attackCooldown;
    }

    public override void ExecuteAttack()
    {
        lastAttackTime = Time.time;
        boss.StopMovement();
        animator.SetTrigger("jumpAttack");

        // Calcular dirección horizontal hacia el jugador
        Vector2 direction = boss.PlayerTransform.position - boss.transform.position;
        float directionX = Mathf.Sign(direction.x); // -1 (izq) o 1 (der)

        rb.linearVelocity = new Vector2(directionX * horizontalForce, rb.linearVelocity.y);
    }



    // Eventos desde animación:
    public void EnableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);

            Vector2 direction = (boss.PlayerTransform.position - transform.position).normalized;
            direction = new Vector2(Mathf.Sign(direction.x), 0f); // Horizontal knockback

            BossAttackHitbox hitbox = attackHitbox.GetComponent<BossAttackHitbox>();
            if (hitbox != null)
            {
                hitbox.SetKnockbackDirection(direction);
            }
        }
    }

    public void DisableHitbox()
    {
        attackHitbox.SetActive(false);
        boss.ResumeMovement();
    }
}