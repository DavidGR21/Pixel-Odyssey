using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject loadingScreen; // Asigna el objeto LoadingScreen desde el inspector
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (loadingScreen != null)
            {
                Debug.Log("[GameManager] loadingScreen encontrado en: " + loadingScreen.scene.name + " - " + loadingScreen.name);
                DontDestroyOnLoad(loadingScreen);
            }
        }
        else
        {
            Destroy(gameObject);
        }
        if (FindObjectsOfType<PersistenceController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private string GetHierarchyPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
    public void PlayGame()
    {
        int profileId = ProfileManager.Instance.ActiveProfileId;
        var persistence = FindObjectOfType<PersistenceController>();
        if (persistence == null)
        {
            Debug.LogError("[GameManager] PersistenceController no encontrado en PlayGame");
            return;
        }

        string bootstrapScene = "MainScene";
        string sceneToLoad = bootstrapScene;

        // Intenta cargar los datos del perfil
        PlayerData data = persistence.LoadGameData();
        if (data != null && !string.IsNullOrEmpty(data.CurrentScene))
        {
            sceneToLoad = data.CurrentScene;
        }
        else
        {
            // Si no hay datos, crea un nuevo PlayerData para este perfil
            Debug.Log("[GameManager] No hay datos guardados, creando datos por defecto para el perfil " + profileId);
            data = new PlayerData
            {
                ProfileId = profileId,
                ProfileName = $"Perfil {profileId}",
                Health = 100f,
                Shield = 50f,
                CurrentScene = bootstrapScene,
                PositionX = 0f,
                PositionY = 0f,
                PositionZ = 0f
            };
            persistence.SaveGameData(data);
            sceneToLoad = bootstrapScene;
        }

        Debug.Log("Escena a cargar: " + sceneToLoad);
        StartCoroutine(PlayAndLoadWithBootstrap(bootstrapScene, sceneToLoad));
    }

    private IEnumerator PlayAndLoadWithBootstrap(string bootstrapScene, string targetScene)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        Debug.Log("Iniciando carga de escena...");
        Debug.Log("Escena objetivo: " + targetScene);
        Debug.Log(bootstrapScene == targetScene);

        if (bootstrapScene == targetScene)
        {
            Debug.Log("La escena guardada es la bootstrap, cargando solo una vez.");
            SceneManager.LoadScene(bootstrapScene, LoadSceneMode.Single);
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(false);
            }
            yield break;
        }
        Debug.Log(bootstrapScene + " " + LoadSceneMode.Single);

        // 1. Carga la escena bootstrap primero
        var op1 = SceneManager.LoadSceneAsync(bootstrapScene, LoadSceneMode.Single);
        yield return op1;
        Debug.Log("Escena bootstrap cargada: " + bootstrapScene);

        // 2. Espera un frame para que los objetos persistentes se inicialicen
        yield return null;
        Debug.Log("Frame de espera tras bootstrap.");

        // 3. Carga la escena guardada
        var op2 = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        yield return op2;
        Debug.Log("Escena guardada cargada: " + targetScene);

        // 4. Espera hasta que el jugador esté en la escena (máximo 2 segundos)
        GameObject player = null;
        float timer = 0f;
        while (player == null && timer < 2f)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
        Debug.Log(player != null ? "Jugador encontrado tras esperar." : "Jugador NO encontrado tras esperar.");

        // 5. Obtén los datos del perfil activo después de cargar la escena
        var persistence = FindObjectOfType<PersistenceController>();
        PlayerData data = null;
        if (persistence != null)
        {
            data = persistence.LoadGameData();
        }
        Debug.Log("persistence =" + (persistence != null));
        Debug.Log("Datos del juego cargados: " + (data != null ? data.CurrentScene : "null"));
        if (player != null && data != null)
        {
            player.transform.position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
            Debug.Log("[GameManager] Posición del jugador restaurada: " + player.transform.position);

            // Restaurar salud y escudo
            var healthPlayer = player.GetComponent<HealthPlayer>();
            if (healthPlayer != null)
            {
                healthPlayer.RestoreHealthAndShield(data.Health, data.Shield);
                Debug.Log("[GameManager] Salud y escudo restaurados: " + data.Health + " / " + data.Shield);
            }
            else
            {
                Debug.LogWarning("[GameManager] No se encontró el componente HealthPlayer en el jugador.");
            }
        }
        else if (player == null)
        {
            Debug.LogWarning("[GameManager] No se encontró el jugador para posicionar después de esperar.");
        }
        else if (data == null)
        {
            Debug.LogWarning("[GameManager] No se encontraron datos del perfil después de cargar la escena.");
        }

    }
}