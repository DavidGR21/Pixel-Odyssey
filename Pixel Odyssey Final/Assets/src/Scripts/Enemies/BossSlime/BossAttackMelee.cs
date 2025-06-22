// BossAttackMelee.cs
using UnityEngine;

public class BossAttackMelee : BossAttackBase
{
    [Header("Condiciones del Ataque")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 3f;

    [Header("Collider del ataque")]
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private Transform hitboxPosition;
    [SerializeField] private float hitboxRadius = 1f;

    private float lastAttackTime = -999f;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // Rango de activación del ataque
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Hitbox visible
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
        if (distance > attackRange) return false;

        var mp = boss.PlayerTransform.GetComponent<MovementPlayer>();
        if (mp == null || !mp.inFloor) return false;

        return Time.time >= lastAttackTime + attackCooldown;
    }

    public override void ExecuteAttack()
    {
        lastAttackTime = Time.time;
        boss.StopMovement();
        animator.SetTrigger("meleeAttack");
    }

    // Animation Events
    public void EnableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);

            // Calcular knockback hacia el jugador según su posición
            Vector2 direction = (boss.PlayerTransform.position - transform.position).normalized;
            direction = new Vector2(Mathf.Sign(direction.x), 0f); // Horizontal únicamente

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