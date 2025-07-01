using UnityEngine;
using System.Collections;

public class Mushroom : Enemy, IMeleeEnemy
{
    public int damage;
    public GameObject hitCollider;
    public GameObject rangeCollider;

    private IEnemyAnimator enemyAnimator;
    private bool isAttacking;
    private float attackCooldown = 2.0f;
    private float attackCooldownTimer = 0.0f;
    private float hurtCooldown = 1.0f;
    private float hurtCooldownTimer = 0.0f;
    private bool isStunned = false;
    private float stunDuration = 0.5f;

    public float AttackRange => attackRange;
    public int Damage => damage;
    public bool IsAttacking => isAttacking;

    public override IEnemyAnimator GetAnimator() => enemyAnimator;

    public override void Initialize()
    {
        base.Initialize();
        enemyAnimator = GetComponent<IEnemyAnimator>();
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<EnemyAnimatorAdapter>();
        }
        target = GameObject.FindWithTag("Player");
    
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
        hurtCooldownTimer = 0.0f;
    }

    public override void UpdateBehavior()
    {
        // Bloquea behaviors si está herido (usa el flag de la clase base Enemy)
        if (isHurtActive)
        {
            return;
        }

        if (target == null)
        {
            return;
        }

        float distanceToPlayerX = Mathf.Abs(transform.position.x - target.transform.position.x);
        float distanceToPlayerY = Mathf.Abs(transform.position.y - target.transform.position.y);


        // 1. Si el jugador NO está en rango X o Y, patrulla
        if (distanceToPlayerX > visionRange || distanceToPlayerY > 3f)
        {
            if (!(currentBehavior is PatrolBehavior))
            {
                SetBehavior(new PatrolBehavior());
            }
        }
        // 2. Si está en rango X y Y, pero fuera de ataque, persigue
        else if (distanceToPlayerX > attackRange)
        {
            if (!(currentBehavior is ChaseBehavior))
            {
                SetBehavior(new ChaseBehavior());
            }
        }
        // 3. Si está en rango X y Y, y dentro de ataque, ataca
        else
        {
            if (!(currentBehavior is AttackBehavior))
            {
                SetBehavior(new AttackBehavior());
            }
        }

        if (currentBehavior == null)
        {
            Debug.Log($"{gameObject.name}: Sin comportamiento asignado (condiciones no cumplidas)");
        }
        else
        {
            currentBehavior.Execute(this);
        }
    }

    // Métodos requeridos por IMeleeEnemy
    public void Attack()
    {
        if (enemyAnimator == null)
        {
            return;
        }
        enemyAnimator.PlayWalk(false);
        enemyAnimator.PlayRun(false);
        enemyAnimator.PlayAttack(true);
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
        enemyAnimator.PlayAttack(false);
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
                // Resetea el flag de daño cada vez que se habilita el collider
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
        if (currentHealth > 0)
        {
            hurtCooldownTimer = hurtCooldown;
        }
        
    }
}