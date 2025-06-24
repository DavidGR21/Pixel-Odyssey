    using UnityEngine;

    public class atackPlayer : MonoBehaviour
    {
        [SerializeField] private Transform ControllerAttack;
        [SerializeField] private Transform groundAttackPoint;
        [SerializeField] private Transform airAttackPoint;
        [SerializeField] private float radioAttack;
        [SerializeField] private float damageAttack;
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private float timeNextAttack;

        private Animator animator;
        private MovementPlayer movementScript;

        [HideInInspector] public bool canMove = true;

        private void Start()
        {
            animator = GetComponent<Animator>();
            movementScript = GetComponent<MovementPlayer>();
        }

        void Update()
        {
            if (timeNextAttack > 0)
            {
                timeNextAttack -= Time.deltaTime;
            }

            if (Input.GetButtonDown("Attack") && timeNextAttack <= 0)
            {
                Attack();
                timeNextAttack = timeBetweenAttacks;
            }
        }

    private void Attack()
    {
        canMove = false;

        bool isGrounded = movementScript != null && movementScript.inFloor;
        ControllerAttack.position = isGrounded ? groundAttackPoint.position : airAttackPoint.position;

        if (isGrounded)
            animator.SetTrigger("Attack");
        else
            animator.SetTrigger("AirAttack");

        // ▶️ Reproducir sonido de ataque
        movementScript?.audioHandler?.PlayAttackSound();
    }


    // Llamado por la animación en el momento del impacto
    public void DoDamage()
        {
            Collider2D[] objects = Physics2D.OverlapCircleAll(ControllerAttack.position, radioAttack);
            foreach (Collider2D obj in objects)
            {
                if (obj.CompareTag("Enemy"))
                {
                    Enemy enemyScript = obj.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        Vector2 knockbackDir = obj.transform.position - transform.position;
                        enemyScript.TakeDamage(damageAttack, knockbackDir, 20f);
                        Debug.Log($"Jugador infligió {damageAttack} de daño al enemigo.");
                    }
                }
            }
        }

        public void EndAttack()
        {
            canMove = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (Application.isPlaying)
            {
                if (ControllerAttack != null)
                    Gizmos.DrawWireSphere(ControllerAttack.position, radioAttack);
            }
            else
            {
                if (groundAttackPoint != null)
                    Gizmos.DrawWireSphere(groundAttackPoint.position, radioAttack);

                if (airAttackPoint != null)
                    Gizmos.DrawWireSphere(airAttackPoint.position, radioAttack);
            }
        }
    }