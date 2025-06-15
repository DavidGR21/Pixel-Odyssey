using Assets.src.Scripts.Scenas.ChangeScene;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] public string sceneToLoad;
    [SerializeField] public string spawnPointName;

    private Animator transitionAnimator;
    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            PlayerSpawnManager.nextSpawnPoint = spawnPointName;

            // Lanza la animación de salida
            transitionAnimator = GameObject.Find("SceneTransition")?.GetComponent<Animator>();
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger("Close");
                // Espera 1 segundo antes de cargar la escena (ajusta al tiempo de tu animación)
                Invoke(nameof(ChangeScene), 1f);
            }
            else
            {
                // Si no hay animación, cambia inmediatamente
                ChangeScene();
            }
        }
    }

    private void ChangeScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawnPoint = GameObject.Find(PlayerSpawnManager.nextSpawnPoint);

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }

        // Actualiza el confiner de la cámara
        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        GameObject newBounds = GameObject.Find("ConfinerBounds");

        if (confiner != null && newBounds != null)
        {
            Collider2D collider = newBounds.GetComponent<Collider2D>();
            if (collider != null)
            {
                confiner.BoundingShape2D = collider;
                confiner.InvalidateBoundingShapeCache();
            }
        }

        // Lanza animación de entrada
        transitionAnimator = GameObject.Find("SceneTransition")?.GetComponent<Animator>();
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Open");
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        isTransitioning = false;
    }
}
