using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar que el jugador colisiona con el objeto TutorialTrigger
        if (other.CompareTag("Player"))
        {
            // Pasamos el mensaje deseado al ShowTutorial
            TutorialUI.Instance.ShowTutorial("Este es un espacio libre, donde podrás practicar tus habilidades para el juego.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Verificar que el jugador salga de la zona y esconder el tutorial
        if (other.CompareTag("Player"))
        {
            TutorialUI.Instance.HideTutorial();
        }
    }
}