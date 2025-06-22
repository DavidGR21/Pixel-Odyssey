using UnityEngine;

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
    public void SaveLevel(string spawnPoint)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        Vector3 pos = player.transform.position;
        HealthPlayer healthPlayer = player.GetComponent<HealthPlayer>();
        float vidaActual = healthPlayer != null ? healthPlayer.CurrentHealth : 0f;
        float escudoActual = healthPlayer != null ? healthPlayer.CurrentShield : 0f;
        var data = new PlayerData
        {
            LastSpawnPoint = spawnPoint,
            PositionX = pos.x,
            PositionY = pos.y,
            PositionZ = pos.z,
            Health = vidaActual,
            Shield = escudoActual
        };
        saveGame.Execute(data);
        Debug.Log("Ruta de guardado: " + Application.persistentDataPath);
        Debug.Log("Progreso guardado automáticamente en el portal.");
    }
}