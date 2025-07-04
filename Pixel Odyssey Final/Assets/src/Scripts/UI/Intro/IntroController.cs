using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Controlador de la introducción del juego.
/// Este script se encarga de reproducir un sonido al inicio y cargar la pantalla de carga después de un tiempo determinado.
/// Debe ser asignado a un GameObject en la escena de introducción.
/// </summary>
public class IntroController : MonoBehaviour
{
    public float duracionLogo = 7f;
    public AudioSource sonidoInicio;

    void Start()
    {
        Invoke("ReproducirSonido", duracionLogo - 5f);
        Invoke("CargarPantallaCarga", duracionLogo);
    }

    void ReproducirSonido()
    {
        if (sonidoInicio != null)
            sonidoInicio.Play();
    }

    void CargarPantallaCarga()
    {
        SceneManager.LoadScene("Loading");
    }
}
