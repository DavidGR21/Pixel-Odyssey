using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private AudioClip startSound;

    public static MenuInicial instance { get; private set; }

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
