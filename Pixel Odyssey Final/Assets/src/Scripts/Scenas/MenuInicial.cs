using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private AudioClip startSound;

    public void PlayGame()
    {
        // Busca el PersistenceController en la escena
        var persistence = FindObjectOfType<PersistenceController>();
        string sceneToLoad = "MainScene"; // Valor por defecto

        if (persistence != null)
        {
            var data = persistence.LoadGameData(); // Nuevo método que retorna PlayerData o null
            if (data != null && !string.IsNullOrEmpty(data.LastSpawnPoint))
            {
                sceneToLoad = data.LastSpawnPoint;
            }
        }

        StartCoroutine(PlayAndLoad(sceneToLoad));
    }

    private IEnumerator PlayAndLoad(string sceneName)
    {
        AudioManager.Instance.PlaySound(startSound);
        yield return new WaitForSeconds(startSound.length);
        SceneManager.LoadScene(sceneName);
    }

    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
