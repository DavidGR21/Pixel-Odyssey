using UnityEngine;
using System.Collections;

public class Goblin : Enemy, IMeleeEnemy
{
    [SerializeField] private int damage;
    [SerializeField] private GameObject hitCollider;
    [SerializeField] private GameObject rangeCollider;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float stunDuration = 0.5f;

    private IEnemyAnimator enemyAnimator;
    private bool isAttacking;
    private float attackCooldownTimer = 0.0f;
    public bool IsAttacking => isAttacking;
    public int Damage => damage;
    public IEnemyAnimator GetAnimator() => enemyAnimator;
    public override void Initialize()
    {
        base.Initialize();
        enemyAnimator = animatorController.GetAnimator() ?? GetComponent<EnemyAnimatorAdapter>();
        if (enemyAnimator == null)
        {
            Debug.LogError("IEnemyAnimator not found on " + gameObject.name);
        }

        if (rangeCollider == null)
        {
            rangeCollider = transform.Find("Range")?.gameObject;
        }
        if (hitCollider == null)
        {
            hitCollider = transform.Find("Hit")?.gameObject;
        }
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;
        }
        isAttacking = false;
        attackCooldownTimer = 0.0f;
    }


    public override void UpdateBehavior()
    {
        if (health.IsHurtActive || isStunned)
        {
            Debug.Log($"{gameObject.name} is hurt or stunned, skipping behavior update.");
            return;
        }

        if (Target == null)
        {
            Debug.LogWarning($"{gameObject.name}: Target is null, cannot update behavior.");
            return;
        }

        float distanceToPlayerX = Mathf.Abs(transform.position.x - Target.transform.position.x);
        float distanceToPlayerY = Mathf.Abs(transform.position.y - Target.transform.position.y);
        Debug.Log($"{gameObject.name}: Distance to player X={distanceToPlayerX}, Y={distanceToPlayerY}, VisionRange={VisionRange}, AttackRange={AttackRange}");

        // 1. If the player is not in X or Y range, patrol
        if (distanceToPlayerX > VisionRange || distanceToPlayerY > 3f)
        {
            if (!(behaviorController.GetCurrentBehavior() is PatrolBehavior))
            {
                SetBehavior(new PatrolBehavior());
            }
        }
        // 2. If in X and Y range but out of attack range, chase
        else if (distanceToPlayerX > AttackRange)
        {
            if (!(behaviorController.GetCurrentBehavior() is ChaseBehavior))
            {
                SetBehavior(new ChaseBehavior());
            }
        }
        // 3. If in X and Y range and within attack range, attack
        else
        {
            if (!(behaviorController.GetCurrentBehavior() is AttackBehavior))
            {
                SetBehavior(new AttackBehavior());
            }
        }

        if (behaviorController.GetCurrentBehavior() == null)
        {
            Debug.LogWarning($"{gameObject.name}: No behavior assigned (conditions not met)");
        }
        else
        {
            behaviorController.UpdateBehavior();
        }
    }


    // IMeleeEnemy methods
    public void Attack()
    {
        if (enemyAnimator == null)
        {
            return;
        }
        animatorController.PlayWalk(false);
        animatorController.PlayRun(false);
        animatorController.PlayAttack(true);
        isAttacking = true;
        if (rangeCollider != null)
        {
            rangeCollider.GetComponent<BoxCollider2D>().enabled = false;
        }
        attackCooldownTimer = attackCooldown;
    }

    public void StopAttack()
    {
        if (enemyAnimator == null)
        {
            return;
        }
        animatorController.PlayAttack(false);
        isAttacking = false;
        if (rangeCollider != null)
        {
            rangeCollider.GetComponent<BoxCollider2D>().enabled = true;
        }
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void EnableAttackCollider(bool enable)
    {
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = enable;
            if (enable)
            {
                var hitScript = hitCollider.GetComponent<HitEnemy>();
                if (hitScript != null)
                {
                    hitScript.ResetDamage();
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

    public void ColliderWeaponTrue()
    {
        EnableAttackCollider(true);
    }

    public void ColliderWeaponFalse()
    {
        EnableAttackCollider(false);
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        if (isAttacking)
        {
            StopAttack();
        }

        base.TakeDamage(damage, knockbackDirection, knockbackForce);
    }

    protected override void Update()
    {
        base.Update();
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }
}