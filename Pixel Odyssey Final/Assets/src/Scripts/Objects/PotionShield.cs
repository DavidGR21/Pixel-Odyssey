using UnityEngine;
/// <summary>
/// Clase que representa una poción de escudo en el juego.
/// Esta clase hereda de MonoBehaviour y se encarga de manejar la recolección de la poción por parte del jugador.
/// Al recoger la poción, se cura el escudo del jugador una cantidad determinada y se reproduce una animación de recolección.
/// La poción se destruye después de un breve retraso para permitir que la animación se reproduzca correctamente.
/// </summary>
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
