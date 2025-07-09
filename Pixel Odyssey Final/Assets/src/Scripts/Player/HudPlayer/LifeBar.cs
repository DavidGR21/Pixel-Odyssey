using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Clase encargada de gestionar la barra de vida del jugador.
/// Muestra la salud actual del jugador en una barra de imagen rellenable.
/// Se suscribe al evento de cambio de salud del script HealthPlayer para actualizar la barra de vida.
/// </summary>
public class LifeBar : MonoBehaviour
{
    [SerializeField] private Image healthImage; // El componente de imagen (rellenable)
    [SerializeField] private HealthPlayer playerHealth; // Referencia al script de salud

    private void Start()
    {
        if (playerHealth == null)
            Debug.LogError("No se ha asignado el HealthPlayer en LifeBar.");

        if (healthImage == null)
            Debug.LogError("No se ha asignado el Image en LifeBar.");

        // Suscribirse al evento de cambio de salud
        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(float currentHealth)
    {
        float fillAmount = currentHealth / playerHealth.maxHealth;
        healthImage.fillAmount = fillAmount;
    }

    private void OnDestroy()
    {
        // Evitar errores si el objeto se destruye
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthBar;
    }
}
