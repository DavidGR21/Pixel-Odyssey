using UnityEngine;
using System.Collections;

public class Skeleton : Enemy, IMeleeEnemy, IShieldEnemy
{
    [Header("Melee Enemy Properties")]
    public float speedWalk;
    public float speedRun;
    public float visionRange;
    public float attackRange =0.7f;
    public int damage;
    public int health;
    public string enemyName;
    public GameObject target;
    public GameObject hitCollider;
    public Animator animator;
    public GameObject rangeCollider;

    private bool isAttacking;
    private float cronometro;
    private int rutina;
    private int direccion;
    private float attackCooldown = 2.0f;
    private float attackCooldownTimer = 0.0f;
    private float hurtCooldown = 1.0f;
    private float hurtCooldownTimer = 0.0f;

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

    public override void Initialize()
    {
        base.Initialize();
        Debug.Log($"[Skeleton {gameObject.name}] Initialize called");
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"[Skeleton {gameObject.name}] No Animator component found");
            return;
        }
        target = GameObject.Find("Player");
        if (target == null)
        {
            Debug.LogError($"[Skeleton {gameObject.name}] Player not found in scene");
            return;
        }
        if (rangeCollider == null)
        {
            rangeCollider = transform.Find("Range")?.gameObject;
            if (rangeCollider == null)
            {
                Debug.LogError($"[Skeleton {gameObject.name}] rangeCollider not assigned and 'Range' child not found");
            }
            else
            {
                var rangeCol = rangeCollider.GetComponent<BoxCollider2D>();
                rangeCol.isTrigger = true; // Ensure rangeCollider is a trigger
                rangeCol.enabled = true;
                Debug.Log($"[Skeleton {gameObject.name}] Initialize: rangeCollider isTrigger={rangeCol.isTrigger}");
            }
        }
        if (hitCollider == null)
        {
            hitCollider = transform.Find("Hit")?.gameObject;
            if (hitCollider == null)
            {
                Debug.LogError($"[Skeleton {gameObject.name}] hitCollider not assigned and 'Hit' child not found");
            }
            else
            {
                var hitCol = hitCollider.GetComponent<BoxCollider2D>();
                hitCol.enabled = false;
                hitCol.isTrigger = false; // Ensure hitCollider is NOT a trigger to avoid continuous damage
                Debug.Log($"[Skeleton {gameObject.name}] Initialize: hitCollider isTrigger={hitCol.isTrigger}, enabled={hitCol.enabled}");
            }
        }
        isAttacking = false;
        attackCooldownTimer = 0.0f;
        hurtCooldownTimer = 0.0f;
        shieldCooldownTimer = 0.0f;
        hasBlockedFirstHit = false;
        Debug.Log($"[Skeleton {gameObject.name}] Initialized: animator={animator != null}, target={target != null}, rangeCollider={rangeCollider != null}, hitCollider={hitCollider != null}");
    }

    private void Update()
    {
        // Safety check to ensure hitCollider is disabled when not attacking
        if (hitCollider != null && hitCollider.GetComponent<BoxCollider2D>().enabled && !isAttacking)
        {
            Debug.LogWarning($"[Skeleton {gameObject.name}] hitCollider was active while not attacking, forcing disable at frame {Time.frameCount}");
            EnableAttackCollider(false);
        }
        // Log collider states for debugging
        Debug.Log($"[Skeleton {gameObject.name}] Update: hitCollider.enabled={hitCollider?.GetComponent<BoxCollider2D>().enabled}, isAttacking={isAttacking}, isShieldActive={isShieldActive}, isHurt={animator?.GetBool("hurt")}");
    }

    public override void UpdateBehavior()
    {
        Debug.Log($"[Skeleton {gameObject.name}] UpdateBehavior called at frame {Time.frameCount}");
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        if (hurtCooldownTimer > 0)
        {
            hurtCooldownTimer -= Time.deltaTime;
        }
        if (shieldCooldownTimer > 0)
        {
            shieldCooldownTimer -= Time.deltaTime;
        }

        bool isHurt = animator != null && animator.GetBool("hurt");
        if (isHurt)
        {
            Debug.Log($"[Skeleton {gameObject.name}] Hurt state active, pausing behavior");
            animator.SetBool("walk", false);
            animator.SetBool("run", false);
            animator.SetBool("attack", false);
            isAttacking = false;
            EnableAttackCollider(false);
            return;
        }

        if (isShieldActive)
        {
            Debug.Log($"[Skeleton {gameObject.name}] Shield active, pausing behavior");
            animator.SetBool("walk", false);
            animator.SetBool("run", false);
            animator.SetBool("attack", false);
            isAttacking = false;
            EnableAttackCollider(false);
            return;
        }

        Comportamientos();
    }

    private void Comportamientos()
    {
        if (target == null)
        {
            Debug.LogWarning($"[Skeleton {gameObject.name}] Target is null, skipping behavior");
            return;
        }

        float distanceToPlayer = Mathf.Abs(transform.position.x - target.transform.position.x);
        Debug.Log($"[Skeleton {gameObject.name}] Distance to player: {distanceToPlayer}, visionRange: {visionRange}, attackRange: {attackRange}, isAttacking: {isAttacking}, attackParam: {animator.GetBool("attack")}, attackCooldownTimer: {attackCooldownTimer}, hurtCooldownTimer: {hurtCooldownTimer}, shieldCooldownTimer: {shieldCooldownTimer}");

        if (isAttacking && distanceToPlayer > attackRange)
        {
            Debug.Log($"[Skeleton {gameObject.name}] Player out of attack range during attack, stopping attack");
            StopAttack();
        }

        if (distanceToPlayer > visionRange && !isAttacking)
        {
            Debug.Log($"[Skeleton {gameObject.name}] Entering Patrol state");
            Patrol();
        }
        else if (distanceToPlayer <= visionRange && distanceToPlayer > attackRange && !isAttacking)
        {
            Debug.Log($"[Skeleton {gameObject.name}] Entering Chase state");
            Chase();
        }
        else if (distanceToPlayer <= attackRange && !isAttacking && attackCooldownTimer <= 0 && hurtCooldownTimer <= 0)
        {
            Debug.Log($"[Skeleton {gameObject.name}] Entering Attack state");
            Attack();
        }
    }

    private void Patrol()
    {
        Debug.Log($"[Skeleton {gameObject.name}] Patrol: cronometro={cronometro}, rutina={rutina}");
        animator.SetBool("run", false);
        cronometro += Time.deltaTime;
        if (cronometro >= 4)
        {
            rutina = Random.Range(0, 2);
            cronometro = 0;
            Debug.Log($"[Skeleton {gameObject.name}] Patrol: New rutina={rutina}");
        }
        switch (rutina)
        {
            case 0:
                animator.SetBool("walk", false);
                Debug.Log($"[Skeleton {gameObject.name}] Patrol: Stop walking");
                break;
            case 1:
                direccion = Random.Range(0, 2);
                rutina++;
                Debug.Log($"[Skeleton {gameObject.name}] Patrol: Set direccion={direccion}, advancing to rutina={rutina}");
                break;
            case 2:
                transform.rotation = Quaternion.Euler(0, direccion == 0 ? 0 : 180, 0);
                transform.Translate(Vector3.right * speedWalk * Time.deltaTime);
                animator.SetBool("walk", true);
                Debug.Log($"[Skeleton {gameObject.name}] Patrol: Moving in direction={direccion}, walk=true");
                break;
        }
    }

    private void Chase()
    {
        Debug.Log($"[Skeleton {gameObject.name}] Chase: Player at x={target.transform.position.x}, Skeleton at x={transform.position.x}");
        transform.rotation = Quaternion.Euler(0, transform.position.x < target.transform.position.x ? 0 : 180, 0);
        animator.SetBool("walk", false);
        animator.SetBool("run", true);
        animator.SetBool("attack", false);
        transform.Translate(Vector3.right * speedRun * Time.deltaTime);
        Debug.Log($"[Skeleton {gameObject.name}] Chase: run=true, attack=false, moving at speed={speedRun}");
    }

    public void Attack()
    {
        Debug.Log($"[Skeleton {gameObject.name}] Attack: isAttacking={isAttacking}, rangeCollider={rangeCollider != null}, hitCollider={hitCollider != null}, attackParam={animator.GetBool("attack")} at frame {Time.frameCount}");
        if (animator == null) return;
        animator.SetBool("walk", false);
        animator.SetBool("run", false);
        animator.SetBool("attack", true);
        isAttacking = true;
        if (rangeCollider != null)
        {
            rangeCollider.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log($"[Skeleton {gameObject.name}] Attack: rangeCollider disabled");
        }
        attackCooldownTimer = attackCooldown;
        // Do NOT enable hitCollider here; let the animation event handle it
    }

    public void StopAttack()
    {
        Debug.Log($"[Skeleton {gameObject.name}] StopAttack: Setting isAttacking=false at frame {Time.frameCount}");
        if (animator == null) return;
        animator.SetBool("attack", false);
        animator.SetBool("run", false); // Ensure run is disabled
        isAttacking = false;
        if (rangeCollider != null)
        {
            rangeCollider.GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log($"[Skeleton {gameObject.name}] StopAttack: rangeCollider enabled");
        }
        EnableAttackCollider(false); // Ensure hitCollider is disabled
    }

    public void EnableAttackCollider(bool enable)
    {
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = enable;
            Debug.Log($"[Skeleton {gameObject.name}] EnableAttackCollider: hitCollider enabled={enable}, isTrigger={hitCollider.GetComponent<BoxCollider2D>().isTrigger} at frame {Time.frameCount}");
        }
        else
        {
            Debug.LogWarning($"[Skeleton {gameObject.name}] EnableAttackCollider: hitCollider is null");
        }
    }

    public void Final_Ani()
    {
        Debug.Log($"[Skeleton {gameObject.name}] Final_Ani: Attack animation ended at frame {Time.frameCount}");
        StopAttack();
    }

    public void ColliderWeaponTrue()
    {
        Debug.Log($"[Skeleton {gameObject.name}] ColliderWeaponTrue: Enabling attack collider at frame {Time.frameCount}");
        EnableAttackCollider(true);
    }

    public void ColliderWeaponFalse()
    {
        Debug.Log($"[Skeleton {gameObject.name}] ColliderWeaponFalse: Disabling attack collider at frame {Time.frameCount}");
        EnableAttackCollider(false);
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        Debug.Log($"[Skeleton {gameObject.name}] TakeDamage called with damage={damage}, knockbackDirection={knockbackDirection}, knockbackForce={knockbackForce} at frame {Time.frameCount}");

        if (isAttacking)
        {
            Debug.Log($"[Skeleton {gameObject.name}] Cancelling attack due to taking damage");
            StopAttack();
        }

        float adjustedDamage;
        if (TakeShieldedDamage(damage, out adjustedDamage))
        {
            if (!hasBlockedFirstHit)
            {
                Debug.Log($"[Skeleton {gameObject.name}] Blocking first hit");
                hasBlockedFirstHit = true;
            }
            else
            {
                Debug.Log($"[Skeleton {gameObject.name}] Blocking attack, shield cooldown available");
            }
            StartCoroutine(ActivateShieldTemporarily());
            return;
        }

        base.TakeDamage(adjustedDamage, knockbackDirection, knockbackForce);
        if (currentHealth > 0)
        {
            hurtCooldownTimer = hurtCooldown;
            Debug.Log($"[Skeleton {gameObject.name}] Damage taken, hurtCooldownTimer set to {hurtCooldown}");
        }
    }

    private System.Collections.IEnumerator ActivateShieldTemporarily()
    {
        isShieldActive = true;
        if (animator != null)
        {
            animator.SetBool("shield", true);
            animator.SetBool("attack", false);
            isAttacking = false;
            EnableAttackCollider(false);
        }
        Debug.Log($"[Skeleton {gameObject.name}] Shield activated temporarily at frame {Time.frameCount}");
        yield return new WaitForSeconds(shieldDuration);
        isShieldActive = false;
        if (animator != null)
        {
            animator.SetBool("shield", false);
        }
        shieldCooldownTimer = shieldCooldown;
        Debug.Log($"[Skeleton {gameObject.name}] Shield deactivated, cooldown of {shieldCooldown} seconds started at frame {Time.frameCount}");
    }

    public void ActivateShield() { }

    public void DeactivateShield() { }

    public bool TakeShieldedDamage(float damage, out float adjustedDamage)
    {
        if (!hasBlockedFirstHit || (shieldCooldownTimer <= 0 && !isShieldActive))
        {
            adjustedDamage = 0f;
            Debug.Log($"[Skeleton {gameObject.name}] Shield active, damage blocked: {damage} to {adjustedDamage} at frame {Time.frameCount}");
            return true;
        }
        adjustedDamage = damage;
        Debug.Log($"[Skeleton {gameObject.name}] Shield not active, taking full damage: {adjustedDamage} at frame {Time.frameCount}");
        return false;
    }
}