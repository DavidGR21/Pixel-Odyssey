using UnityEngine;
/// <summary>
/// Clase encargada de gestionar el final del tutorial.
/// Muestra un mensaje de finalización del tutorial cuando el jugador entra en el área de colisión.
/// Al salir del área, oculta el mensaje.
/// </summary>
public class TutorialEnd : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialUI.Instance.ShowTutorial("¡ERES MUY HABIL! De aquí en adelante disfruta el juego. ¡SUERTE!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialUI.Instance.HideTutorial();
        }
    }
}
