using UnityEngine;
using System.Collections;

public class Skeleton : Enemy, IMeleeEnemy, IShieldEnemy
{
    [Header("Melee Enemy Properties")]
    [SerializeField] private int damage;
    [SerializeField] private GameObject hitCollider;
    [SerializeField] private GameObject rangeCollider;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float hurtCooldown = 1.0f;

    private IEnemyAnimator enemyAnimator;
    private IShieldEnemyAnimator shieldAnimator;
    private bool isAttacking;
    private float attackCooldownTimer = 0.0f;
    private float hurtCooldownTimer = 0.0f;
    private bool isStunned = false;

    [Header("Shield Properties")]
    private bool isShieldActive;
    [SerializeField] private float shieldCooldown = 4.0f;
    private float shieldCooldownTimer;
    [SerializeField] private float shieldDuration = 0.5f;
    private bool hasBlockedFirstHit;

    public int Damage => damage;
    public bool IsAttacking => isAttacking;
    public bool IsShieldActive => isShieldActive;

    public override void Initialize()
    {
        base.Initialize();

        // Asigna animadores desde el controlador
        enemyAnimator = AnimatorController.GetAnimator() ?? GetComponent<EnemyAnimatorAdapter>();
        shieldAnimator = GetComponent<IShieldEnemyAnimator>();

        if (rangeCollider == null)
            rangeCollider = transform.Find("Range")?.gameObject;
        if (hitCollider == null)
            hitCollider = transform.Find("Hit")?.gameObject;
        if (hitCollider != null)
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;

        isAttacking = false;
        attackCooldownTimer = 0.0f;
        hurtCooldownTimer = 0.0f;
        shieldCooldownTimer = 0.0f;
        hasBlockedFirstHit = false;
    }

    protected override void Update()
    {
        base.Update();
        // Seguridad: desactiva el hitCollider si no está atacando
        if (hitCollider != null && hitCollider.GetComponent<BoxCollider2D>().enabled && !isAttacking)
        {
            EnableAttackCollider(false);
        }
    }

    public override void UpdateBehavior()
    {
        // Actualiza timers
        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;
        if (hurtCooldownTimer > 0)
            hurtCooldownTimer -= Time.deltaTime;
        if (shieldCooldownTimer > 0)
            shieldCooldownTimer -= Time.deltaTime;

        // Bloquea behaviors si está herido o escudo activo
        bool isHurt = enemyAnimator != null && enemyAnimator.IsHurt();
        if (health.IsHurtActive || isShieldActive)
        {
            if (BehaviorController.GetCurrentBehavior() != null)
                SetBehavior(null);
            return;
        }

        if (Target == null)
        {
            if (BehaviorController.GetCurrentBehavior() != null)
                SetBehavior(null);
            return;
        }

        float distanceToPlayerX = Mathf.Abs(transform.position.x - Target.transform.position.x);
        float distanceToPlayerY = Mathf.Abs(transform.position.y - Target.transform.position.y);

        // 1. Si el jugador NO está en rango X o Y, patrulla
        if (distanceToPlayerX > VisionRange || distanceToPlayerY > 3f)
        {
            if (!(BehaviorController.GetCurrentBehavior() is PatrolBehavior))
            {
                SetBehavior(new PatrolBehavior());
            }
        }
        // 2. Si está en rango X y Y, pero fuera de ataque, persigue
        else if (distanceToPlayerX > AttackRange)
        {
            if (!(BehaviorController.GetCurrentBehavior() is ChaseBehavior))
            {
                SetBehavior(new ChaseBehavior());
            }
        }
        // 3. Si está en rango X y Y, y dentro de ataque, ataca
        else
        {
            if (!(BehaviorController.GetCurrentBehavior() is AttackBehavior))
            {
                SetBehavior(new AttackBehavior());
            }
        }

        if (BehaviorController.GetCurrentBehavior() != null)
        {
            BehaviorController.GetCurrentBehavior().Execute(this);
        }
    }

    // Métodos requeridos por IMeleeEnemy
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
            if (enable)
            {
                // Llama a ResetDamage SOLO en HitEnemigo2D
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

    // Estos métodos deben ser llamados desde los eventos de animación
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
            StopAttack();

        float adjustedDamage;
        // Bloqueo de daño por escudo
        if (TakeShieldedDamage(damage, out adjustedDamage))
        {
            if (!hasBlockedFirstHit)
            {
                hasBlockedFirstHit = true;
            }
            StartCoroutine(ActivateShieldTemporarily());
            // Ejecuta animación de "hurt" aunque el daño sea bloqueado
            if (enemyAnimator != null)
            {
                enemyAnimator.PlayHurt(true);
                StartCoroutine(ResetHurtAnimation());
            }
            return;
        }

        base.TakeDamage(adjustedDamage, knockbackDirection, knockbackForce);
        if (health.CurrentHealth > 0)
            hurtCooldownTimer = hurtCooldown;
    }

    private IEnumerator ResetHurtAnimation()
    {
        yield return new WaitForSeconds(0.2f); // Ajusta el tiempo según tu animación
        if (enemyAnimator != null)
            enemyAnimator.PlayHurt(false);
    }

    private IEnumerator ActivateShieldTemporarily()
    {
        isShieldActive = true;
        if (shieldAnimator != null)
            shieldAnimator.PlayShield(true); // Activa animación de escudo

        if (enemyAnimator != null)
        {
            enemyAnimator.PlayAttack(false);
            isAttacking = false;
            EnableAttackCollider(false);
        }
        yield return new WaitForSeconds(shieldDuration);
        isShieldActive = false;
        if (shieldAnimator != null)
            shieldAnimator.PlayShield(false); // Desactiva animación de escudo
        shieldCooldownTimer = shieldCooldown;
    }

    public void ActivateShield() { }
    public void DeactivateShield() { }

    // Lógica de escudo: true si bloquea el daño, false si recibe daño real
    public bool TakeShieldedDamage(float damage, out float adjustedDamage)
    {
        if (!hasBlockedFirstHit || (shieldCooldownTimer <= 0 && !isShieldActive))
        {
            adjustedDamage = 0f;
            return true;
        }

        adjustedDamage = damage;
        return false;
    }

    public void InjectAnimators(IEnemyAnimator animator, IShieldEnemyAnimator shieldAnimator)
    {
        this.enemyAnimator = animator;
        this.shieldAnimator = shieldAnimator;
    }
}

