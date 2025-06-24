using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private AudioClip startSound;

    public void PlayGame()
    {
        Debug.Log("Cargando escena de selección de perfil...");
        SceneManager.LoadScene("Profiles", LoadSceneMode.Single); // Cambia "Profiles" por el nombre exacto de tu escena de perfiles
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
