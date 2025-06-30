using UnityEngine;
using TMPro;

public class TutorialObjAttack : MonoBehaviour
{
    public static TutorialObjAttack Instance { get; private set; }

    [Header("Referencia al Panel del Tutorial")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText; // Aquí se encuentra el texto que se mostrará

    private bool isShowing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Asegurarse de que el panel esté desactivado al inicio
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isShowing) // Si el jugador colisiona con el objeto
        {
            ShowTutorial();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isShowing) // Cuando el jugador se aleja
        {
            HideTutorial();
        }
    }

    // Mostrar el tutorial
    public void ShowTutorial()
    {
        if (tutorialPanel != null && !isShowing)
        {
            tutorialPanel.SetActive(true);
            isShowing = true;

            // Configura el texto
            if (tutorialText != null)
            {
                tutorialText.text = "Puedes atacar a los enemigos con X y con C puedes moverte más rápido a los lados.";
                tutorialText.fontSize = 30;
                tutorialText.color = Color.white;
            }

        }
    }

    // Ocultar el tutorial
    public void HideTutorial()
    {
        if (tutorialPanel != null && isShowing)
        {
            tutorialPanel.SetActive(false);
            isShowing = false;

        }
    }
}
