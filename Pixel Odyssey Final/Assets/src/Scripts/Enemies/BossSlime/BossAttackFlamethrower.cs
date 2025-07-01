using UnityEngine;
using Assets.src.Scripts.Enemies.BossSlime;
/// <summary>
/// Clase que representa el ataque de lanzallamas del jefe Slime.
/// </summary>
public class BossAttackFlamethrower : BossAttackBase
{
    [Header("Condiciones del Ataque")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 5f;

    [Header("Collider del ataque")]
    [SerializeField] private GameObject flamethrowerHitbox;
    [SerializeField] private Transform hitboxPosition;
    [SerializeField] private float hitboxRadius = 1.5f;

    private float lastAttackTime = -999f;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // Rango de activación del ataque
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Hitbox visible
            if (hitboxPosition != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(hitboxPosition.position, hitboxRadius);
            }
        }
    }

    public override bool CanAttack()
    {
        if (!boss.IsTransformed() || Time.time < lastAttackTime + attackCooldown)
            return false;

        float distance = Mathf.Abs(boss.PlayerTransform.position.x - transform.position.x);
        if (distance > attackRange) return false;

        MovementPlayer mp = boss.PlayerTransform.GetComponent<MovementPlayer>();
        if (mp == null || mp.inFloor) return false; // Solo si está en el aire

        return true;
    }

    public override void ExecuteAttack()
    {
        lastAttackTime = Time.time;
        boss.StopMovement();
        animator.SetTrigger("flamethrowerAttack"); // Asegúrate de que exista este trigger
    }

    // Llamados desde la animación
    public void EnableHitbox() => flamethrowerHitbox.SetActive(true);
    public void DisableHitbox()
    {
        flamethrowerHitbox.SetActive(false);
        boss.ResumeMovement();
    }
}