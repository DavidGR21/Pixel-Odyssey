using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
/// <summary>
/// Clase encargada de gestionar el estado del juego y la carga de escenas.
/// Permite iniciar el juego, restaurar el estado desde la persistencia y manejar la pantalla de carga.
/// Utiliza un patrón singleton para asegurar que solo haya una instancia de GameManager en la escena.
/// </summary>
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
            return;
        }

        PlayerData data = persistence.LoadGameData();
        if (data == null || string.IsNullOrEmpty(data.CurrentScene))
        {
            Debug.LogWarning("[GameManager] No hay datos guardados para restaurar.");
            return;
        }

        bool spawnPoint = !string.IsNullOrEmpty(data.SpawnPointName);

        StartCoroutine(PlayAndLoadWithBootstrap("MainScene", data.CurrentScene, spawnPoint));
    }
    public void PlayGame()
    {
        int profileId = ProfileManager.Instance.ActiveProfileId;
        var persistence = FindObjectOfType<PersistenceController>();
        if (persistence == null)
        {
            return;
        }

        string bootstrapScene = "MainScene";
        string sceneToLoad = bootstrapScene;
        bool spawnPoint = false;
        // Intenta cargar los datos del perfil
        PlayerData data = persistence.LoadGameData();
        if (data != null && !string.IsNullOrEmpty(data.CurrentScene))
        {
            spawnPoint = !string.IsNullOrEmpty(data.SpawnPointName);
            sceneToLoad = data.CurrentScene;
        }
        else
        {
            // Si no hay datos, crea un nuevo PlayerData para este perfil
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

        StartCoroutine(PlayAndLoadWithBootstrap(bootstrapScene, sceneToLoad, spawnPoint));
    }

    private IEnumerator PlayAndLoadWithBootstrap(string bootstrapScene, string targetScene, bool spawnPoint)
    {
        // 1. Instancia la pantalla de carga y hazla persistente
        GameObject loadingScreen = null;
        if (loadingScreenPrefab != null)
        {
            loadingScreen = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(loadingScreen);
        }
        else
        {
            Debug.LogWarning("[LoadingScreen] No se asignó el prefab de pantalla de carga.");
        }

        if (bootstrapScene == targetScene && (spawnPoint == false))
        {
            SceneManager.LoadScene(bootstrapScene, LoadSceneMode.Single);
            if (loadingScreen != null)
            {
                Destroy(loadingScreen);
            }
            yield break;
        }

        // 2. Carga la escena bootstrap primero
        var op1 = SceneManager.LoadSceneAsync(bootstrapScene, LoadSceneMode.Single);
        yield return op1;

        // 3. Espera un frame para que los objetos persistentes se inicialicen
        yield return null;

        // 4. Carga la escena guardada
        var op2 = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        yield return op2;

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

        // 6. Obtén los datos del perfil activo después de cargar la escena
        var persistence = FindObjectOfType<PersistenceController>();
        PlayerData data = null;
        if (persistence != null)
        {
            data = persistence.LoadGameData();
        }
        if (player != null && data != null)
        {
            player.transform.position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);

            // Restaurar salud y escudo
            var healthPlayer = player.GetComponent<HealthPlayer>();
            if (healthPlayer != null)
            {
                healthPlayer.RestoreHealthAndShield(data.Health, data.Shield);
                healthPlayer.Revive(data.Health, data.Shield);

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
        }
    }

    public GameObject loadingScreenPrefab; // Asigna el prefab en el inspector


}