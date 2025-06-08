using Assets.src.Scripts.Scenas.ChangeScene;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] public string sceneToLoad;
    [SerializeField] public string spawnPointName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSpawnManager.nextSpawnPoint = spawnPointName;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawnPoint = GameObject.Find(PlayerSpawnManager.nextSpawnPoint);

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }

        // Buscar la Cinemachine Virtual Camera y actualizar el confiner
        CinemachineConfiner2D confiner = FindObjectOfType<CinemachineConfiner2D>();
        GameObject newBounds = GameObject.Find("ConfinerBounds");

        if (confiner != null && newBounds != null)
        {
            Collider2D collider = newBounds.GetComponent<Collider2D>();
            if (collider != null)
            {
                confiner.BoundingShape2D = collider;
                confiner.InvalidateCache(); // Necesario para que se actualicen los límites
            }
        }

        // Limpia el evento
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
