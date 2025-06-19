using UnityEngine;
using System.Collections;

public class Goblin : Enemy, IMeleeEnemy
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
            Debug.Log($"{gameObject.name}: enemyAnimator no encontrado, intentando EnemyAnimatorAdapter");
        }
        target = GameObject.FindWithTag("Player");
        if (target != null)
        {
            Debug.Log($"{gameObject.name}: Jugador detectado en {target.transform.position}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Jugador no encontrado");
        }
        if (rangeCollider == null)
        {
            rangeCollider = transform.Find("Range")?.gameObject;
            Debug.Log($"{gameObject.name}: rangeCollider {(rangeCollider != null ? "encontrado" : "no encontrado")}");
        }
        if (hitCollider == null)
        {
            hitCollider = transform.Find("Hit")?.gameObject;
            Debug.Log($"{gameObject.name}: hitCollider {(hitCollider != null ? "encontrado" : "no encontrado")}");
        }
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log($"{gameObject.name}: hitCollider inicializado como deshabilitado");
        }
        isAttacking = false;
        attackCooldownTimer = 0.0f;
        hurtCooldownTimer = 0.0f;
        Debug.Log($"{gameObject.name}: Inicializado con attackCooldownTimer={attackCooldownTimer}, hurtCooldownTimer={hurtCooldownTimer}");
    }

    public override void UpdateBehavior()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            Debug.Log($"{gameObject.name}: attackCooldownTimer={attackCooldownTimer}");
        }
        if (hurtCooldownTimer > 0)
        {
            hurtCooldownTimer -= Time.deltaTime;
            Debug.Log($"{gameObject.name}: hurtCooldownTimer={hurtCooldownTimer}");
        }

        // Selección de estrategia según el estado
        if (isStunned)
        {
            SetBehavior(null);
            Debug.Log($"{gameObject.name}: Estado=Stunned, comportamiento establecido a null");
        }
        else if (enemyAnimator.IsHurt())
        {
            SetBehavior(null);
            Debug.Log($"{gameObject.name}: Estado=Hurt, comportamiento establecido a null");
        }
        else if (target == null)
        {
            SetBehavior(null);
            Debug.Log($"{gameObject.name}: Jugador no detectado, comportamiento establecido a null");
        }
        else
        {
            float distanceToPlayer = Mathf.Abs(transform.position.x - target.transform.position.x);
            Debug.Log($"{gameObject.name}: Distancia al jugador={distanceToPlayer}, visionRange={visionRange}, attackRange={attackRange}");

            if (isAttacking && distanceToPlayer > attackRange)
            {
                StopAttack();
                Debug.Log($"{gameObject.name}: Jugador fuera de rango de ataque, deteniendo ataque");
            }

            if (distanceToPlayer > visionRange && !isAttacking)
            {
                SetBehavior(new PatrolBehavior());
                Debug.Log($"{gameObject.name}: Ejecutando PatrolBehavior (fuera de visionRange)");
            }
            else if (distanceToPlayer <= visionRange && distanceToPlayer > attackRange && !isAttacking)
            {
                SetBehavior(new ChaseBehavior());
                Debug.Log($"{gameObject.name}: Ejecutando ChaseBehavior (dentro de visionRange, fuera de attackRange)");
            }
            else if (distanceToPlayer <= attackRange && !isAttacking && attackCooldownTimer <= 0 && hurtCooldownTimer <= 0)
            {
                SetBehavior(new AttackBehavior());
                Debug.Log($"{gameObject.name}: Ejecutando AttackBehavior (dentro de attackRange)");
            }
            else
            {
                SetBehavior(null);
                Debug.Log($"{gameObject.name}: Sin comportamiento asignado (condiciones no cumplidas)");
            }
        }

        base.UpdateBehavior();
        Debug.Log($"{gameObject.name}: UpdateBehavior ejecutado, comportamiento actual={(currentBehavior != null ? currentBehavior.GetType().Name : "null")}");
    }

    // Métodos requeridos por IMeleeEnemy
    public void Attack()
    {
        if (enemyAnimator == null)
        {
            Debug.LogWarning($"{gameObject.name}: Intento de ataque, pero enemyAnimator es null");
            return;
        }
        enemyAnimator.PlayWalk(false);
        enemyAnimator.PlayRun(false);
        enemyAnimator.PlayAttack(true);
        isAttacking = true;
        Debug.Log($"{gameObject.name}: Iniciando ataque, isAttacking={isAttacking}");
        if (rangeCollider != null)
        {
            rangeCollider.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log($"{gameObject.name}: rangeCollider deshabilitado durante ataque");
        }
        attackCooldownTimer = attackCooldown;
        Debug.Log($"{gameObject.name}: attackCooldownTimer reiniciado a {attackCooldown}");
    }

    public void StopAttack()
    {
        if (enemyAnimator == null)
        {
            Debug.LogWarning($"{gameObject.name}: Intento de detener ataque, pero enemyAnimator es null");
            return;
        }
        enemyAnimator.PlayAttack(false);
        isAttacking = false;
        Debug.Log($"{gameObject.name}: Ataque detenido, isAttacking={isAttacking}");
        if (rangeCollider != null)
        {
            rangeCollider.GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log($"{gameObject.name}: rangeCollider habilitado");
        }
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log($"{gameObject.name}: hitCollider deshabilitado");
        }
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
        Debug.Log($"{gameObject.name}: Final_Ani llamado, iniciando stun después del ataque");
        StartCoroutine(StunAfterAttack());
    }

    private IEnumerator StunAfterAttack()
    {
        isStunned = true;
        Debug.Log($"{gameObject.name}: Iniciando stun, isStunned={isStunned}, duración={stunDuration}");
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        Debug.Log($"{gameObject.name}: Stun terminado, isStunned={isStunned}");
    }

    public void ColliderWeaponTrue()
    {
        EnableAttackCollider(true);
        Debug.Log($"{gameObject.name}: ColliderWeaponTrue llamado");
    }

    public void ColliderWeaponFalse()
    {
        EnableAttackCollider(false);
        Debug.Log($"{gameObject.name}: ColliderWeaponFalse llamado");
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        if (isAttacking)
        {
            StopAttack();
            Debug.Log($"{gameObject.name}: Ataque interrumpido por daño recibido");
        }

        base.TakeDamage(damage, knockbackDirection, knockbackForce);
        if (currentHealth > 0)
        {
            hurtCooldownTimer = hurtCooldown;
            Debug.Log($"{gameObject.name}: Daño recibido, hurtCooldownTimer={hurtCooldownTimer}, salud actual={currentHealth}");
        }
        else
        {
            Debug.Log($"{gameObject.name}: Salud agotada, enemigo destruido");
        }
    }
}