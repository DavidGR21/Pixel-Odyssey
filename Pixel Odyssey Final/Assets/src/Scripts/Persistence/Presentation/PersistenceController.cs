using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceController : MonoBehaviour
{
    private SaveGame saveGame;
    private LoadGame loadGame;

    private void Awake()
    {
        var repo = RepositoryFactory.Create();
        saveGame = new SaveGame(repo);
        loadGame = new LoadGame(repo);
    }

    public void Save()
    {
        var data = new PlayerData { Health = 100 };
        saveGame.Execute(data);
    }

    public void Load()
    {
        var data = loadGame.Execute();
        if (data != null)
        {
            // Debug.Log($"Cargando nivel guardado: {data.CurrentLevel}");
            // Si CurrentLevel es el índice de la escena:
            // SceneManager.LoadScene(data.CurrentLevel);
            // Si CurrentLevel es el nombre de la escena, usa:
            // SceneManager.LoadScene("NombreDeLaEscena");
        }
        else
        {
            Debug.Log("No hay datos guardados.");
        }
    }
    public PlayerData LoadGameData()
    {
        return loadGame.Execute();
    }
    public void SaveLevel(string spawnPointName)
    {
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        Vector3 pos = Vector3.zero;

        if (spawnPoint != null)
        {
            pos = spawnPoint.transform.position;
        }
        else
        {
            // Si no se encuentra el spawnPoint, usa la posición actual del jugador como fallback
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                pos = player.transform.position;
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        HealthPlayer healthPlayer = playerObj != null ? playerObj.GetComponent<HealthPlayer>() : null;
        float vidaActual = healthPlayer != null ? healthPlayer.CurrentHealth : 0f;
        float escudoActual = healthPlayer != null ? healthPlayer.CurrentShield : 0f;

        var data = new PlayerData
        {
            CurrentScene = SceneManager.GetActiveScene().name,
            PositionX = pos.x,
            PositionY = pos.y,
            PositionZ = pos.z,
            Health = vidaActual,
            Shield = escudoActual
        };
        saveGame.Execute(data);
        Debug.Log("Ruta de guardado: " + Application.persistentDataPath);
        Debug.Log("Progreso guardado automáticamente en el portal (posición del spawnPoint).");
    }

    public PlayerData GetProfileData(int profileId)
    {
        // Asume que tu LoadGame y repositorio ya soportan perfiles
        return loadGame.Execute(profileId);
    }

    public string GetProfileName(int profileId)
    {
        var data = GetProfileData(profileId);
        return data != null ? data.ProfileName : $"Perfil {profileId}";
    }

    public float GetProfileHealth(int profileId)
    {
        var data = GetProfileData(profileId);
        return data != null ? data.Health : 0f;
    }

    public float GetProfileShield(int profileId)
    {
        var data = GetProfileData(profileId);
        return data != null ? data.Shield : 0f;
    }

    public string GetProfileScene(int profileId)
    {
        var data = GetProfileData(profileId);
        return data != null ? data.CurrentScene : "Sin partida";
    }
}