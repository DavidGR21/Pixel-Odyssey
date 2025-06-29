using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceController : MonoBehaviour
{
    private SaveGame saveGame;
    private LoadGame loadGame;

    private void Awake()
    {
        Debug.Log("[PersistenceController] Awake en: " + gameObject.scene.name + " - " + gameObject.name);
        if (FindObjectsOfType<PersistenceController>().Length > 1)
        {
            Debug.LogWarning("[PersistenceController] Duplicado, se destruye: " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        var unitOfWork = new UnitOfWork();
        saveGame = new SaveGame(unitOfWork);
        loadGame = new LoadGame(unitOfWork);
    }

    // Guarda el estado actual del perfil activo (puedes llamar esto para un guardado rápido)
    public void Save()
    {
        int profileId = ProfileManager.Instance != null ? ProfileManager.Instance.ActiveProfileId : 1;
        var data = new PlayerData
        {
            ProfileId = profileId,
            ProfileName = $"Perfil {profileId}",
            Health = 100, // Puedes personalizar estos valores por defecto
            Shield = 50,
            CurrentScene = SceneManager.GetActiveScene().name,
            PositionX = 0f,
            PositionY = 0f,
            PositionZ = 0f
        };
        saveGame.Execute(data);
    }

    // Carga los datos del perfil activo
    public PlayerData LoadGameData()
    {
        int profileId = ProfileManager.Instance != null ? ProfileManager.Instance.ActiveProfileId : 1;
        return loadGame.Execute(profileId);
    }

    // Guarda el estado actual del jugador en el perfil activo (usado por portales, checkpoints, etc.)
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
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                pos = player.transform.position;
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        HealthPlayer healthPlayer = playerObj != null ? playerObj.GetComponent<HealthPlayer>() : null;
        float vidaActual = healthPlayer != null ? healthPlayer.CurrentHealth : 0f;
        float escudoActual = healthPlayer != null ? healthPlayer.CurrentShield : 0f;

        int profileId = ProfileManager.Instance != null ? ProfileManager.Instance.ActiveProfileId : 1;
        Debug.Log("Act " + spawnPointName + " sc " + SceneManager.GetActiveScene().name);
        var data = new PlayerData
        {
            ProfileId = profileId,
            SpawnPointName = spawnPointName,
            ProfileName = $"Perfil {profileId}",
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

    // Guarda cualquier PlayerData personalizado (por ejemplo, al iniciar una partida nueva)
    public void SaveGameData(PlayerData data)
    {
        if (data.ProfileId == 0)
            data.ProfileId = ProfileManager.Instance != null ? ProfileManager.Instance.ActiveProfileId : 1;
        if (string.IsNullOrEmpty(data.ProfileName))
            data.ProfileName = $"Perfil {data.ProfileId}";
        saveGame.Execute(data);
    }

    // Métodos para obtener info de cualquier perfil (para la UI)
    public PlayerData GetProfileData(int profileId)
    {
        return loadGame.Execute(profileId);
    }

    public string GetProfileName(int profileId)
    {
        var data = GetProfileData(profileId);
        return data != null ? data.ProfileName : $"Perfil {profileId}";
    }

    public string GetProfileHealth(int profileId)
    {
        var data = GetProfileData(profileId);
        if (data == null)
            return "-";
        if (data.Health == 0f)
            return "-";
        return data.Health.ToString();
    }
    public float GetProfileShield(int profileId)
    {
        var data = GetProfileData(profileId);
        return data != null ? data.Shield : 0f;
    }
    //aqui se devuelve el nombre de la escena del spawnPoint porque es la escena donde aparece el jugador

    public string GetProfileScene(int profileId)
    {
        var data = GetProfileData(profileId);
        if (data == null)
            return "Sin Datos";
        if (string.IsNullOrEmpty(data.SpawnPointName))
            return "Inicio";
        return data.SpawnPointName;
    }
}