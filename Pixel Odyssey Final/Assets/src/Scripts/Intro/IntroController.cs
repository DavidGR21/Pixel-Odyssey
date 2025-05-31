using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public float duracionLogo = 2f; // segundos que dura el logo

    void Start()
    {
        Invoke("CargarPantallaCarga", duracionLogo);
    }

    void CargarPantallaCarga()
    {
        SceneManager.LoadScene("Loading");
    }
}
