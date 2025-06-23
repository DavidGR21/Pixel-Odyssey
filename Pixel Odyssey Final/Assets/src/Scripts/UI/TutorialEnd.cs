using UnityEngine;

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
