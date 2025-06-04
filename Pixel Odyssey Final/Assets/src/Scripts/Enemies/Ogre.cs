using UnityEngine;

public class Ogre : Enemy, IMeleeEnemy
{
    public float speedWalk;
    public float speedRun;
    public float visionRange;
    public float attackRange;
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

    public float AttackRange => attackRange;
    public int Damage => damage;
    public bool IsAttacking => isAttacking;

    public override void Initialize()
    {
        animator = GetComponent<Animator>();
        target = GameObject.Find("Player");
        isAttacking = false;
    }

    public override void UpdateBehavior()
    {
        Comportamientos();
    }

    private void Comportamientos()
    {
        if (Mathf.Abs(transform.position.x - target.transform.position.x) > visionRange && !isAttacking)
        {
            Patrol();
        }
        else if (Mathf.Abs(transform.position.x - target.transform.position.x) > attackRange && !isAttacking)
        {
            Chase();
        }
        else
        {
            Attack();
        }
    }

    private void Patrol()
    {
        animator.SetBool("run", false);
        cronometro += Time.deltaTime;
        if (cronometro >= 4)
        {
            rutina = Random.Range(0, 2);
            cronometro = 0;
        }
        switch (rutina)
        {
            case 0:
                animator.SetBool("walk", false);
                break;
            case 1:
                direccion = Random.Range(0, 2);
                rutina++;
                break;
            case 2:
                transform.rotation = Quaternion.Euler(0, direccion == 0 ? 0 : 180, 0);
                transform.Translate(Vector3.right * speedWalk * Time.deltaTime);
                animator.SetBool("walk", true);
                break;
        }
    }

    private void Chase()
    {
        transform.rotation = Quaternion.Euler(0, transform.position.x < target.transform.position.x ? 0 : 180, 0);
        animator.SetBool("walk", false);
        animator.SetBool("run", true);
        animator.SetBool("attack", false);
        transform.Translate(Vector3.right * speedRun * Time.deltaTime);
    }

    public void Attack()
    {
        animator.SetBool("walk", false);
        animator.SetBool("run", false);
        animator.SetBool("attack", true);
        isAttacking = true;
        rangeCollider.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void StopAttack()
    {
        animator.SetBool("attack", false);
        isAttacking = false;
        rangeCollider.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void EnableAttackCollider(bool enable)
    {
        hitCollider.GetComponent<BoxCollider2D>().enabled = enable;
    }

    // Métodos llamados por animaciones
    public void Final_Ani()
    {
        StopAttack();
    }

    public void ColliderWeaponTrue()
    {
        EnableAttackCollider(true);
    }   

    public void ColliderWeaponFalse()
    {
        EnableAttackCollider(false);
    }
}