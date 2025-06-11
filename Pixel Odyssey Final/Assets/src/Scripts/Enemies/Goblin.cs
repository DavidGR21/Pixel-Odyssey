using UnityEngine;
using System.Collections;

public class Goblin : Enemy, IMeleeEnemy
{
    public float speedWalk;
    public float speedRun;
    public float visionRange;
    public float attackRange = 0.7f;
    public int damage;
    public int health;
    public string enemyName;
    public GameObject target;
    public GameObject hitCollider;
    public GameObject rangeCollider;

    private IEnemyAnimator enemyAnimator;
    private bool isAttacking;
    private float cronometro;
    private int rutina;
    private int direccion;
    private float attackCooldown = 2.0f;
    private float attackCooldownTimer = 0.0f;
    private float hurtCooldown = 1.0f;
    private float hurtCooldownTimer = 0.0f;
    private bool isStunned = false;
    private float stunDuration = 0.5f;

    public float AttackRange => attackRange;
    public int Damage => damage;
    public bool IsAttacking => isAttacking;

    public override void Initialize()
    {
        base.Initialize();
        enemyAnimator = GetComponent<IEnemyAnimator>();
        if (enemyAnimator == null)
            enemyAnimator = GetComponent<EnemyAnimatorAdapter>();
        target = GameObject.FindWithTag("Player");
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

    public override void UpdateBehavior()
    {
        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;
        if (hurtCooldownTimer > 0)
            hurtCooldownTimer -= Time.deltaTime;

        if (isStunned)
        {
            enemyAnimator.PlayWalk(false);
            enemyAnimator.PlayRun(false);
            return;
        }

        if (enemyAnimator.IsHurt())
        {
            enemyAnimator.PlayWalk(false);
            enemyAnimator.PlayRun(false);
            return;
        }

        Comportamientos();
    }

    private void Comportamientos()
    {
        if (target == null) return;

        float distanceToPlayer = Mathf.Abs(transform.position.x - target.transform.position.x);

        if (isAttacking && distanceToPlayer > attackRange)
            StopAttack();

        if (distanceToPlayer > visionRange && !isAttacking)
            Patrol();
        else if (distanceToPlayer <= visionRange && distanceToPlayer > attackRange && !isAttacking)
            Chase();
        else if (distanceToPlayer <= attackRange && !isAttacking && attackCooldownTimer <= 0 && hurtCooldownTimer <= 0)
            Attack();
    }

    private void Patrol()
    {
        enemyAnimator.PlayRun(false);
        cronometro += Time.deltaTime;
        if (cronometro >= 4)
        {
            rutina = Random.Range(0, 2);
            cronometro = 0;
        }
        switch (rutina)
        {
            case 0:
                enemyAnimator.PlayWalk(false);
                break;
            case 1:
                direccion = Random.Range(0, 2);
                rutina++;
                break;
            case 2:
                transform.rotation = Quaternion.Euler(0, direccion == 0 ? 0 : 180, 0);
                transform.Translate(Vector3.right * speedWalk * Time.deltaTime);
                enemyAnimator.PlayWalk(true);
                break;
        }
    }

    private void Chase()
    {
        transform.rotation = Quaternion.Euler(0, transform.position.x < target.transform.position.x ? 0 : 180, 0);
        enemyAnimator.PlayWalk(false);
        enemyAnimator.PlayRun(true);
        enemyAnimator.PlayAttack(false);
        transform.Translate(Vector3.right * speedRun * Time.deltaTime);
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
            hitCollider.GetComponent<BoxCollider2D>().enabled = enable;
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
        if (currentHealth > 0)
            hurtCooldownTimer = hurtCooldown;
    }
}