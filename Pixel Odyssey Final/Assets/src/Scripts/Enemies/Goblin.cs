using UnityEngine;

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

    public float AttackRange => attackRange;
    public int Damage => damage;
    public bool IsAttacking => isAttacking;
    
    public override void Initialize()
    {
        base.Initialize();
        Debug.Log($"[Goblin {gameObject.name}] Initialize called");
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"[Goblin {gameObject.name}] No Animator component found");
            return;
        }
        target = GameObject.Find("Player");
        if (target == null)
        {
            Debug.LogError($"[Goblin {gameObject.name}] Player not found in scene");
            return;
        }
        if (rangeCollider == null)
        {
            rangeCollider = transform.Find("Range")?.gameObject;
            if (rangeCollider == null)
            {
                Debug.LogError($"[Goblin {gameObject.name}] rangeCollider not assigned and 'Range' child not found");
            }
        }
        if (hitCollider == null)
        {
            hitCollider = transform.Find("Hit")?.gameObject;
            if (hitCollider == null)
            {
                Debug.LogError($"[Goblin {gameObject.name}] hitCollider not assigned and 'Hit' child not found");
            }
        }
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log($"[Goblin {gameObject.name}] Initialize: hitCollider disabled");
        }
        isAttacking = false;
        attackCooldownTimer = 0.0f;
        hurtCooldownTimer = 0.0f;
        Debug.Log($"[Goblin {gameObject.name}] Initialized: animator={animator != null}, target={target != null}, rangeCollider={rangeCollider != null}, hitCollider={hitCollider != null}");
    }

    public override void UpdateBehavior()
    {
        Debug.Log($"[Goblin {gameObject.name}] UpdateBehavior called");
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        if (hurtCooldownTimer > 0)
        {
            hurtCooldownTimer -= Time.deltaTime;
        }

        // Verificar si el estado hurt está activo
        bool isHurt = animator != null && animator.GetBool("hurt");
        if (isHurt)
        {
            Debug.Log($"[Goblin {gameObject.name}] Hurt state active, pausing behavior");
            // Detener animaciones de movimiento
            animator.SetBool("walk", false);
            animator.SetBool("run", false);
            return; // Salir para evitar ejecutar Comportamientos()
        }

        Comportamientos();
    }

    private void Comportamientos()
    {
        if (target == null)
        {
            Debug.LogWarning($"[Goblin {gameObject.name}] Target is null, skipping behavior");
            return;
        }

        float distanceToPlayer = Mathf.Abs(transform.position.x - target.transform.position.x);
        Debug.Log($"[Goblin {gameObject.name}] Distance to player: {distanceToPlayer}, visionRange: {visionRange}, attackRange: {attackRange}, isAttacking: {isAttacking}, attackParam: {animator.GetBool("attack")}, attackCooldownTimer: {attackCooldownTimer}, hurtCooldownTimer: {hurtCooldownTimer}");

        if (isAttacking && distanceToPlayer > attackRange)
        {
            Debug.Log($"[Goblin {gameObject.name}] Player out of attack range during attack, stopping attack");
            StopAttack();
        }

        if (distanceToPlayer > visionRange && !isAttacking)
        {
            Debug.Log($"[Goblin {gameObject.name}] Entering Patrol state");
            Patrol();
        }
        else if (distanceToPlayer <= visionRange && distanceToPlayer > attackRange && !isAttacking)
        {
            Debug.Log($"[Goblin {gameObject.name}] Entering Chase state");
            Chase();
        }
        else if (distanceToPlayer <= attackRange && !isAttacking && attackCooldownTimer <= 0 && hurtCooldownTimer <= 0)
        {
            Debug.Log($"[Goblin {gameObject.name}] Entering Attack state");
            Attack();
        }
        else
        {
            Debug.Log($"[Goblin {gameObject.name}] Cannot attack: isAttacking={isAttacking}, attackCooldownTimer={attackCooldownTimer}, hurtCooldownTimer={hurtCooldownTimer}, distance={distanceToPlayer}");
        }
    }

    private void Patrol()
    {
        Debug.Log($"[Goblin {gameObject.name}] Patrol: cronometro={cronometro}, rutina={rutina}");
        animator.SetBool("run", false);
        cronometro += Time.deltaTime;
        if (cronometro >= 4)
        {
            rutina = Random.Range(0, 2);
            cronometro = 0;
            Debug.Log($"[Goblin {gameObject.name}] Patrol: New rutina={rutina}");
        }
        switch (rutina)
        {
            case 0:
                animator.SetBool("walk", false);
                Debug.Log($"[Goblin {gameObject.name}] Patrol: Stop walking");
                break;
            case 1:
                direccion = Random.Range(0, 2);
                rutina++;
                Debug.Log($"[Goblin {gameObject.name}] Patrol: Set direccion={direccion}, advancing to rutina={rutina}");
                break;
            case 2:
                transform.rotation = Quaternion.Euler(0, direccion == 0 ? 0 : 180, 0);
                transform.Translate(Vector3.right * speedWalk * Time.deltaTime);
                animator.SetBool("walk", true);
                Debug.Log($"[Goblin {gameObject.name}] Patrol: Moving in direction={direccion}, walk=true");
                break;
        }
    }

    private void Chase()
    {
        Debug.Log($"[Goblin {gameObject.name}] Chase: Player at x={target.transform.position.x}, Goblin at x={transform.position.x}");
        transform.rotation = Quaternion.Euler(0, transform.position.x < target.transform.position.x ? 0 : 180, 0);
        animator.SetBool("walk", false);
        animator.SetBool("run", true);
        animator.SetBool("attack", false);
        transform.Translate(Vector3.right * speedRun * Time.deltaTime);
        Debug.Log($"[Goblin {gameObject.name}] Chase: run=true, attack=false, moving at speed={speedRun}");
    }

    public void Attack()
    {
        Debug.Log($"[Goblin {gameObject.name}] Attack: isAttacking={isAttacking}, rangeCollider={rangeCollider != null}, hitCollider={hitCollider != null}, attackParam={animator.GetBool("attack")}");
        if (animator == null) return;
        animator.SetBool("walk", false);
        animator.SetBool("run", false);
        animator.SetBool("attack", true);
        isAttacking = true;
        if (rangeCollider != null)
        {
            rangeCollider.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log($"[Goblin {gameObject.name}] Attack: rangeCollider disabled");
        }
        else
        {
            Debug.LogWarning($"[Goblin {gameObject.name}] Attack: rangeCollider is null");
        }
        attackCooldownTimer = attackCooldown;
    }

    public void StopAttack()
    {
        Debug.Log($"[Goblin {gameObject.name}] StopAttack: Setting isAttacking=false");
        if (animator == null) return;
        animator.SetBool("attack", false);
        isAttacking = false;
        if (rangeCollider != null)
        {
            rangeCollider.GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log($"[Goblin {gameObject.name}] StopAttack: rangeCollider enabled");
        }
        else
        {
            Debug.LogWarning($"[Goblin {gameObject.name}] StopAttack: rangeCollider is null");
        }
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log($"[Goblin {gameObject.name}] StopAttack: hitCollider disabled");
        }
    }

    public void EnableAttackCollider(bool enable)
    {
        if (hitCollider != null)
        {
            hitCollider.GetComponent<BoxCollider2D>().enabled = enable;
            Debug.Log($"[Goblin {gameObject.name}] EnableAttackCollider: hitCollider enabled={enable}, isTrigger={hitCollider.GetComponent<BoxCollider2D>().isTrigger}");
        }
        else
        {
            Debug.LogWarning($"[Goblin {gameObject.name}] EnableAttackCollider: hitCollider is null");
        }
    }

    public void Final_Ani()
    {
        Debug.Log($"[Goblin {gameObject.name}] Final_Ani: Attack animation ended");
        StopAttack();
    }

    public void ColliderWeaponTrue()
    {
        Debug.Log($"[Goblin {gameObject.name}] ColliderWeaponTrue: Enabling attack collider");
        EnableAttackCollider(true);
    }

    public void ColliderWeaponFalse()
    {
        Debug.Log($"[Goblin {gameObject.name}] ColliderWeaponFalse: Disabling attack collider");
        EnableAttackCollider(false);
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        Debug.Log($"[Goblin {gameObject.name}] TakeDamage called with damage={damage}, knockbackDirection={knockbackDirection}, knockbackForce={knockbackForce} at frame {Time.frameCount}");

        // Cancelar el ataque si está atacando
        if (isAttacking)
        {
            Debug.Log($"[Goblin {gameObject.name}] Cancelling attack due to taking damage");
            StopAttack();
        }

        base.TakeDamage(damage, knockbackDirection, knockbackForce);
        if (currentHealth > 0)
        {
            hurtCooldownTimer = hurtCooldown;
            Debug.Log($"[Goblin {gameObject.name}] TakeDamage: hurtCooldownTimer set to {hurtCooldown}");
        }
    }
}