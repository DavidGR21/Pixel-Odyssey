using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuInicial : MonoBehaviour
{

    [SerializeField] private AudioClip startSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
    {
        StartCoroutine(PlayAndLoad());
    }

    private IEnumerator PlayAndLoad() // metodo para esperar antes de cambair dxe escena 
    {
        AudioManager.Instance.PlayUISound(startSound);
        yield return new WaitForSeconds(startSound.length); // espera hasta que se reproduzca el sonido para cambiar de escena
        SceneManager.LoadScene("MainScene");
    }

    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
