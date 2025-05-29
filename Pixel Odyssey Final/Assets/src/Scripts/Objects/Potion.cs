using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] private float healAmount = 20f;
    [SerializeField] private float destroyDelay = 0.5f; // Tiempo de la animación

    private bool isCollected = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        HealthPlayer playerHealth = other.GetComponent<HealthPlayer>();
        if (playerHealth != null)
        {
            isCollected = true;

            // Curar al jugador
            playerHealth.Heal(healAmount);

            // Activar animación al tomar la poción
            if (animator != null)
                animator.SetTrigger("Collect");

            // Desactivar el collider para evitar múltiples contactos
            GetComponent<Collider2D>().enabled = false;

            // Destruir luego de la animación
            Destroy(gameObject, destroyDelay);
        }
    }
}
