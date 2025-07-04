using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
/// <summary>
/// Clase encargada de gestionar el menú inicial del juego.
/// Permite reproducir un sonido de inicio y cargar la escena de selección de perfil.
/// También proporciona métodos para iniciar el juego y salir de la aplicación.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip startSound;

    public static MainMenu instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        StartCoroutine(PlayIntroAndLoadMain());
    }

    private IEnumerator PlayIntroAndLoadMain()
    {
        if (startSound != null)
        {
            AudioManager.Instance.PlayUISound(startSound);
            yield return new WaitForSeconds(startSound.length);
        }

    }

    public void PlayGame()
    {
        Debug.Log("Cargando escena de selección de perfil...");
        SceneManager.LoadScene("Profiles", LoadSceneMode.Single); // Cambia si tu escena tiene otro nombre
    }

    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
