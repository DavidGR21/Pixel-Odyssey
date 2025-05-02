using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
    {
        SceneManager.LoadScene("FirstScene");
    }

    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
