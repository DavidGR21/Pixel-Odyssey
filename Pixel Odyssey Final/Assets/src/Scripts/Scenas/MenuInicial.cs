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

    private IEnumerator PlayAndLoad()
    {
        AudioManager.Instance.PlaySound(startSound);
        yield return new WaitForSeconds(startSound.length); // espera la duración del sonido
        SceneManager.LoadScene("FirstScene");
    }

    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
