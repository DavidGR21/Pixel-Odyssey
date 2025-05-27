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
    private MovementPlayer movementScript; // Referencia al script de movimiento

    [HideInInspector] public bool canMove = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        movementScript = GetComponent<MovementPlayer>(); // Obtener referencia al script de movimiento
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

        // Verificamos si está en el suelo usando la variable pública del script de movimiento
        bool isGrounded = movementScript != null && movementScript.inFloor;

        // Cambiar el punto de ataque
        ControllerAttack.position = isGrounded ? groundAttackPoint.position : airAttackPoint.position;

        // Activar animación según el estado
        if (isGrounded)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            animator.SetTrigger("AirAttack");
        }
    }

    // Este método lo llamará la animación en el momento del impacto
    public void DoDamage()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(ControllerAttack.position, radioAttack);
        foreach (Collider2D obj in objects)
        {
            if (obj.CompareTag("Enemy"))
            {
                obj.GetComponent<enemy>().takeDamage(damageAttack);
            }
        }
    }

    // Este método debe llamarse al final de la animación
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
