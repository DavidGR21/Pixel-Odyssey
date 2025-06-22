using UnityEngine;
using System.Collections;

public class Skeleton : Enemy, IMeleeEnemy, IShieldEnemy
{
    [Header("Melee Enemy Properties")]
    public int damage;
    public GameObject hitCollider;
    public GameObject rangeCollider;

    private IEnemyAnimator enemyAnimator;
    private IShieldEnemyAnimator shieldAnimator;
    private bool isAttacking;
    private float attackCooldown = 2.0f;
    private float attackCooldownTimer = 0.0f;
    private float hurtCooldown = 1.0f;
    private float hurtCooldownTimer = 0.0f;
    private bool isStunned = false;
    private float stunDuration = 0.5f;

    [Header("Shield Properties")]
    private bool isShieldActive;
    private float shieldCooldown = 4.0f;
    private float shieldCooldownTimer;
    private float shieldDuration = 0.5f;
    private bool hasBlockedFirstHit;

    public float AttackRange => attackRange;
    public int Damage => damage;
    public bool IsAttacking => isAttacking;
    public bool IsShieldActive => isShieldActive;

    public override IEnemyAnimator GetAnimator() => enemyAnimator;

    // Nuevo método para inyectar ambos adaptadores
    public void InjectAnimators(IEnemyAnimator animator, IShieldEnemyAnimator shieldAnimator)
    {
        this.enemyAnimator = animator;
        this.shieldAnimator = shieldAnimator;
    }

    public override void Initialize()
    {
        base.Initialize();
        
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
        shieldCooldownTimer = 0.0f;
        hasBlockedFirstHit = false;
    }

    private void Update()
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
        if (isHurt || isShieldActive)
        {
            if (currentBehavior != null)
                SetBehavior(null);
            return;
        }

        if (target == null)
        {
            if (currentBehavior != null)
                SetBehavior(null);
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
                Debug.Log($"{gameObject.name}: Cambiando a Patrulla");
            }
        }
        // 2. Si está en rango X y Y, pero fuera de ataque, persigue
        else if (distanceToPlayerX > attackRange)
        {
            if (!(currentBehavior is ChaseBehavior))
            {
                SetBehavior(new ChaseBehavior());
                Debug.Log($"{gameObject.name}: Cambiando a Persecución");
            }
        }
        // 3. Si está en rango X y Y, y dentro de ataque, ataca
        else
        {
            if (!(currentBehavior is AttackBehavior))
            {
                SetBehavior(new AttackBehavior());
                Debug.Log($"{gameObject.name}: Cambiando a Ataque");
            }
        }

        if (currentBehavior == null)
        {
            Debug.Log($"{gameObject.name}: Sin comportamiento asignado (condiciones no cumplidas)");
        }
        else
        {
            Debug.Log($"{gameObject.name}: Ejecutando comportamiento: {currentBehavior.GetType().Name}");
            currentBehavior.Execute(this);
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
            Debug.Log($"{gameObject.name}: hitCollider {(enable ? "habilitado" : "deshabilitado")}");
            if (enable)
            {
                // Resetea el flag de daño cada vez que se habilita el collider
                var hitScript = hitCollider.GetComponent<HitEnemigo2D>();
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
        if (currentHealth > 0)
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
}