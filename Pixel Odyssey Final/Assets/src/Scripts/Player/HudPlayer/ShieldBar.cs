using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] private Image shieldImage; // Componente de imagen (rellenable)
    [SerializeField] private HealthPlayer playerHealth; // Referencia al script de salud

    private void Start()
    {
        if (playerHealth == null)
            Debug.LogError("No se ha asignado el HealthPlayer en ShieldBar.");

        if (shieldImage == null)
            Debug.LogError("No se ha asignado el Image en ShieldBar.");

        // Suscribirse al evento de cambio de escudo
        playerHealth.OnShieldChanged += UpdateShieldBar;
    }

    private void UpdateShieldBar(float currentShield)
    {
        float fillAmount = currentShield / playerHealth.maxShield;
        shieldImage.fillAmount = fillAmount;
    }

    private void OnDestroy()
    {
        // Evitar errores si el objeto se destruye
        if (playerHealth != null)
            playerHealth.OnShieldChanged -= UpdateShieldBar;
    }
}
