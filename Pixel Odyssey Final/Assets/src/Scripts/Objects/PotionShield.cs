using UnityEngine;

public class PotionSield : MonoBehaviour
{
    [SerializeField] private float healAmountShield = 25f;
    [SerializeField] private float destroyDelay = 0.5f;

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

            // Curar el escudo del jugador
            playerHealth.HealShield(healAmountShield);

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
