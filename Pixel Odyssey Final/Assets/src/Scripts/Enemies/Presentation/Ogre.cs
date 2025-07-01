using UnityEngine;
using System.Collections;

public class Ogre : Enemy, IMeleeEnemy
{
    [SerializeField] private int damage;
    [SerializeField] private GameObject hitCollider;
    [SerializeField] private GameObject rangeCollider;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float hurtCooldown = 1.0f;
    [SerializeField] private float stunDuration = 0.5f;

    private IEnemyAnimator enemyAnimator;
    private bool isAttacking;
    private float attackCooldownTimer = 0.0f;
    private float hurtCooldownTimer = 0.0f;
    private bool isStunned = false;

    public int Damage => damage;
    public bool IsAttacking => isAttacking;

    public IEnemyAnimator GetAnimator() => enemyAnimator;

    public override void Initialize()
    {
        base.Initialize();
        enemyAnimator = AnimatorController.GetAnimator() ?? GetComponent<EnemyAnimatorAdapter>();

        if (rangeCollider == null)
            rangeCollider = transform.Find("Range")?.gameObject;
        if (hitCollider == null)
            hitCollider = transform.Find("Hit")?.gameObject;
        if (hitCollider != null)
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;

        isAttacking = false;
        attackCooldownTimer = 0.0f;
        hurtCooldownTimer = 0.0f;
    }

    public void UpdateBehavior()
    {
        if (health.IsHurtActive || isStunned)
        {
            Debug.Log($"{gameObject.name}: Está herido o aturdido, no ejecuta behaviors.");
            return;
        }
        if (Target == null)
        {
            Debug.Log($"{gameObject.name}: Sin target asignado.");
            return;
        }

        float distanceToPlayerX = Mathf.Abs(transform.position.x - Target.transform.position.x);
        float distanceToPlayerY = Mathf.Abs(transform.position.y - Target.transform.position.y);

        Debug.Log($"{gameObject.name}: Distancia al jugador X={distanceToPlayerX}, Y={distanceToPlayerY}, visionRange={VisionRange}, attackRange={AttackRange}");

        // 1. Si el jugador NO está en rango X o Y, patrulla
        if (distanceToPlayerX > VisionRange || distanceToPlayerY > 3f)
        {
            if (!(BehaviorController.GetCurrentBehavior() is PatrolBehavior))
            {
                SetBehavior(new PatrolBehavior());
                Debug.Log($"{gameObject.name}: Cambiando a Patrulla");
            }
        }
        // 2. Si está en rango X y Y, pero fuera de ataque, persigue
        else if (distanceToPlayerX > AttackRange)
        {
            if (!(BehaviorController.GetCurrentBehavior() is ChaseBehavior))
            {
                SetBehavior(new ChaseBehavior());
                Debug.Log($"{gameObject.name}: Cambiando a Persecución");
            }
        }
        // 3. Si está en rango X y Y, y dentro de ataque, ataca
        else
        {
            if (!(BehaviorController.GetCurrentBehavior() is AttackBehavior))
            {
                SetBehavior(new AttackBehavior());
                Debug.Log($"{gameObject.name}: Cambiando a Ataque");
            }
        }

        if (BehaviorController.GetCurrentBehavior() == null)
        {
            Debug.Log($"{gameObject.name}: Sin comportamiento asignado (condiciones no cumplidas)");
        }
        else
        {
            Debug.Log($"{gameObject.name}: Ejecutando comportamiento: {BehaviorController.GetCurrentBehavior().GetType().Name}");
            BehaviorController.GetCurrentBehavior().Execute(this);
        }
    }

    public void Attack()
    {
        if (enemyAnimator == null) return;
        enemyAnimator.PlayWalk(false);
        enemyAnimator.PlayRun(false);
        enemyAnimator.PlayAttack(true);
        isAttacking = true;
        if (rangeCollider != null)
            rangeCollider.GetComponent<BoxCollider2D>().enabled = false;
        attackCooldownTimer = attackCooldown;
    }

    public void StopAttack()
    {
        if (enemyAnimator == null) return;
        enemyAnimator.PlayAttack(false);
        isAttacking = false;
        if (rangeCollider != null)
            rangeCollider.GetComponent<BoxCollider2D>().enabled = true;
        if (hitCollider != null)
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void EnableAttackCollider(bool enable)
    {
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = enable;
            Debug.Log($"{gameObject.name}: hitCollider {(enable ? "habilitado" : "deshabilitado")}");
            if (enable)
            {
                var hitScript = hitCollider.GetComponent<HitEnemy>();
                if (hitScript != null)
                {
                    hitScript.ResetDamage();
                    Debug.Log($"{gameObject.name}: ResetDamage llamado en HitEnemigo2D");
                }
            }
        }
    }

    public void Final_Ani()
    {
        StopAttack();
        StartCoroutine(StunAfterAttack());
    }

    private IEnumerator StunAfterAttack()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
    }

    public void ColliderWeaponTrue() => EnableAttackCollider(true);
    public void ColliderWeaponFalse() => EnableAttackCollider(false);

    public override void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        if (isAttacking)
            StopAttack();

        base.TakeDamage(damage, knockbackDirection, knockbackForce);
        if (health.CurrentHealth > 0)
            hurtCooldownTimer = hurtCooldown;
    }
}