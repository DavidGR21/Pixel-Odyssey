using UnityEngine;
using UnityEngine.SceneManagement;

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
