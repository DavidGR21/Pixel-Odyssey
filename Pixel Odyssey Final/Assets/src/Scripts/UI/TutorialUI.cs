using UnityEngine;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    public static TutorialUI Instance { get; private set; }

    [Header("Referencia al Panel del Tutorial")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;  // Referencia directa al componente de texto

    private bool isShowing = false;

    void Awake()
    {
        // Asegurarse de que solo haya una instancia de este script (Singleton)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false); // Inicialmente el panel está desactivado
        }
    }

    // Mostrar el tutorial con el mensaje dinámico
    public void ShowTutorial(string message)
    {
        // Si el tutorialPanel y el mensaje no se están mostrando, lo mostramos
        if (tutorialPanel != null && !isShowing)
        {
            tutorialPanel.SetActive(true);
            isShowing = true;

            // Cambiar el texto mostrado en el componente TMP_Text
            if (tutorialText != null)
            {
                tutorialText.text = message;  // Usar el mensaje pasado como argumento
                tutorialText.fontSize = 30;
                tutorialText.color = Color.white;
            }

            Debug.Log("Tutorial mostrado.");
        }
    }
    public void HideTutorial()
    {
        if (tutorialPanel != null && isShowing)
        {
            tutorialPanel.SetActive(false);
            isShowing = false;
            Debug.Log("Tutorial finalizado.");
        }
    }
}
