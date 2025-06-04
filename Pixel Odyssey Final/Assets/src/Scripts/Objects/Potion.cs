using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] private float healAmount = 20f;
    [SerializeField] private float destroyDelay = 0.5f; // Tiempo de la animaci�n

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

            // Activar animaci�n al tomar la poci�n
            if (animator != null)
                animator.SetTrigger("Collect");

            // Desactivar el collider para evitar m�ltiples contactos
            GetComponent<Collider2D>().enabled = false;

            // Destruir luego de la animaci�n
            Destroy(gameObject, destroyDelay);
        }
    }
}
