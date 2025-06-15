using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraConfinerUpdater : MonoBehaviour
{
    private CinemachineConfiner2D confiner;

    void Awake()
    {
        // Persistir este objeto
        DontDestroyOnLoad(gameObject);

        // Escuchar cuando se cargue una nueva escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Buscar el confiner en la cámara
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Buscar el nuevo collider en la escena cargada
        PolygonCollider2D newBounds = FindFirstObjectByType<PolygonCollider2D>();

        if (confiner != null && newBounds != null)
        {
            confiner.BoundingShape2D = newBounds;
            confiner.InvalidateBoundingShapeCache(); // muy importante
            Debug.Log($"Confiner actualizado para la escena: {scene.name}");
        }
        else
        {
            Debug.LogWarning("No se encontró el confiner o el collider en la nueva escena.");
        }
    }

    void OnDestroy()
    {
        // Siempre remover el listener cuando se destruya
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
