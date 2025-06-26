using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

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
    public void RestoreLevelFromPersistence()
    {
        var persistence = FindObjectOfType<PersistenceController>();
        if (persistence == null)
        {
            Debug.LogError("[GameManager] PersistenceController no encontrado en RestoreLevelFromPersistence");
            return;
        }

        PlayerData data = persistence.LoadGameData();
        if (data == null || string.IsNullOrEmpty(data.CurrentScene))
        {
            Debug.LogWarning("[GameManager] No hay datos guardados para restaurar.");
            return;
        }

        Debug.Log("[GameManager] Restaurando nivel: " + data.CurrentScene);
        StartCoroutine(PlayAndLoadWithBootstrap("MainScene", data.CurrentScene));
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
        // 1. Instancia la pantalla de carga y hazla persistente
        GameObject loadingScreen = null;
        Debug.Log("[LoadingScreen] Prefab asignado: " + (loadingScreenPrefab != null));
        if (loadingScreenPrefab != null)
        {
            loadingScreen = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(loadingScreen);
            Debug.Log("[LoadingScreen] Instanciado y marcado como DontDestroyOnLoad: " + loadingScreen.name);
        }
        else
        {
            Debug.LogWarning("[LoadingScreen] No se asignó el prefab de pantalla de carga.");
        }

        Debug.Log("[GameManager] Iniciando carga de escena...");
        Debug.Log("[GameManager] Escena objetivo: " + targetScene);
        Debug.Log("[GameManager] ¿Bootstrap == targetScene?: " + (bootstrapScene == targetScene));

        if (bootstrapScene == targetScene)
        {
            Debug.Log("[GameManager] Cargando solo bootstrapScene.");
            SceneManager.LoadScene(bootstrapScene, LoadSceneMode.Single);
            if (loadingScreen != null)
            {
                Destroy(loadingScreen);
                Debug.Log("[LoadingScreen] Destruido tras cargar bootstrapScene.");
            }
            yield break;
        }

        // 2. Carga la escena bootstrap primero
        Debug.Log("[GameManager] Cargando escena bootstrap: " + bootstrapScene);
        var op1 = SceneManager.LoadSceneAsync(bootstrapScene, LoadSceneMode.Single);
        yield return op1;
        Debug.Log("[GameManager] Escena bootstrap cargada: " + bootstrapScene);

        // 3. Espera un frame para que los objetos persistentes se inicialicen
        yield return null;
        Debug.Log("[GameManager] Frame de espera tras bootstrap.");

        // 4. Carga la escena guardada
        Debug.Log("[GameManager] Cargando escena guardada: " + targetScene);
        var op2 = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        yield return op2;
        Debug.Log("[GameManager] Escena guardada cargada: " + targetScene);

        // 5. Espera hasta que el jugador esté en la escena (máximo 2 segundos)
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
        Debug.Log(player != null ? "[GameManager] Jugador encontrado tras esperar." : "[GameManager] Jugador NO encontrado tras esperar.");

        // 6. Obtén los datos del perfil activo después de cargar la escena
        var persistence = FindObjectOfType<PersistenceController>();
        PlayerData data = null;
        if (persistence != null)
        {
            data = persistence.LoadGameData();
        }
        Debug.Log("[GameManager] persistence =" + (persistence != null));
        Debug.Log("[GameManager] Datos del juego cargados: " + (data != null ? data.CurrentScene : "null"));
        if (player != null && data != null)
        {
            player.transform.position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
            Debug.Log("[GameManager] Posición del jugador restaurada: " + player.transform.position);

            // Restaurar salud y escudo
            var healthPlayer = player.GetComponent<HealthPlayer>();
            if (healthPlayer != null)
            {
                healthPlayer.RestoreHealthAndShield(data.Health, data.Shield);
                healthPlayer.Revive(data.Health, data.Shield);

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

        // 7. Solo aquí, cuando el jugador ya está en su posición final y restaurado, destruye la pantalla de carga
        if (player != null && data != null)
        {
            player.transform.position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
          
        }

        // Espera un frame extra para asegurar que todo se ha actualizado visualmente
        yield return null;

        // Espera 1 segundo extra antes de quitar la pantalla de carga
        yield return new WaitForSeconds(1.2f);

        if (loadingScreen != null)
        {
            Destroy(loadingScreen);
            Debug.Log("[LoadingScreen] Destruido al final del proceso de carga.");
        }
    }

    public GameObject loadingScreenPrefab; // Asigna el prefab en el inspector


}