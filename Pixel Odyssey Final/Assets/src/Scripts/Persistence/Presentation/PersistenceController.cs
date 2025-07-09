using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de persistencia con soporte para usuarios y contraseñas.
/// Mantiene compatibilidad con implementaciones anteriores.
/// </summary>
public class PersistenceController : MonoBehaviour
{
    [Header("Repository Configuration")]
    public RepositoryFactory.RepositoryType repositoryType = RepositoryFactory.RepositoryType.SQLite;
    public DatabaseConfig databaseConfig;

    private SaveGame saveGame;
    private LoadGame loadGame;
    private UnitOfWork unitOfWork;

    private void Awake()
    {
        if (FindObjectsOfType<PersistenceController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        InitializeRepository();
    }

    private void InitializeRepository()
    {
        try
        {
            unitOfWork = new UnitOfWork(repositoryType, databaseConfig);
            saveGame = new SaveGame(unitOfWork);
            loadGame = new LoadGame(unitOfWork);
            
            Debug.Log($"Repositorio inicializado: {repositoryType}");
            
            if (repositoryType == RepositoryFactory.RepositoryType.SQLite)
            {
                string dbPath = System.IO.Path.Combine(Application.persistentDataPath, 
                    databaseConfig?.databaseName ?? "PixelOdyssey.db");
                Debug.Log($"Base de datos SQLite ubicada en: {dbPath}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"Error inicializando repositorio {repositoryType}: {ex.Message}");
            Debug.LogWarning("Usando File repository como fallback.");
            
            unitOfWork = new UnitOfWork(RepositoryFactory.RepositoryType.File);
            saveGame = new SaveGame(unitOfWork);
            loadGame = new LoadGame(unitOfWork);
        }
    }

    private void OnDestroy()
    {
        unitOfWork?.Dispose();
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
    }

    // Guarda cualquier PlayerData personalizado
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
    // Obtiene el nombre del perfil, o un nombre por defecto si no hay datos
        public string GetProfileName(int profileId)
        {
            var data = GetProfileData(profileId);
            return data != null ? data.ProfileName : $"Perfil {profileId}";
        }
    // Obtiene la escena actual del perfil, o un mensaje por defecto si no hay datos
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

        public string GetProfileScene(int profileId)
        {
            var data = GetProfileData(profileId);
            if (data == null)
                return "Sin Datos";
            if (string.IsNullOrEmpty(data.SpawnPointName))
                return "Inicio";
            return data.SpawnPointName;
        }

    // Método adicional para SQLite
    public PlayerData[] LoadAllProfiles()
    {
        if (unitOfWork.GameRepository is SQLiteGameRepository sqliteRepo)
        {
            return sqliteRepo.LoadAllProfiles();
        }
        return new PlayerData[0];
    }

    // Método para debug - agregar al final de la clase
    [ContextMenu("Test Save/Load")]
    public void TestSaveLoad()
    {
        Debug.Log("=== Iniciando test de Save/Load ===");
        
        // Crear datos de prueba
        var testData = new PlayerData
        {
            ProfileId = 999,
            ProfileName = "Test Profile",
            Health = 75f,
            Shield = 25f,
            CurrentScene = "TestScene",
            SpawnPointName = "TestSpawn",
            PositionX = 10f,
            PositionY = 5f,
            PositionZ = 0f
        };
        
        // Guardar
        Debug.Log("Guardando datos de prueba...");
        saveGame.Execute(testData);
        
        // Cargar
        Debug.Log("Cargando datos de prueba...");
        var loadedData = loadGame.Execute(999);
        
        if (loadedData != null)
        {
            Debug.Log($"✅ ÉXITO: Datos cargados correctamente");
            Debug.Log($"ProfileId: {loadedData.ProfileId}");
            Debug.Log($"ProfileName: {loadedData.ProfileName}");
            Debug.Log($"Health: {loadedData.Health}");
            Debug.Log($"Scene: {loadedData.CurrentScene}");
        }
        else
        {
            Debug.Log("❌ ERROR: No se pudieron cargar los datos");
        }
        
        Debug.Log("=== Fin del test ===");
    }

    /// <summary>
    /// Cambia la contraseña de un perfil de usuario.
    /// </summary>
    public bool ChangePassword(int profileId, string currentPassword, string newPassword, out string errorMessage)
    {
        errorMessage = "";

        try
        {
            Debug.Log($"🔧 ChangePassword iniciado - ProfileId: {profileId}");
            
            // Validar nueva contraseña
            if (!PasswordHelper.IsValidPassword(newPassword, out errorMessage))
            {
                Debug.Log($"❌ Contraseña inválida: {errorMessage}");
                return false;
            }

            // Cargar datos del perfil
            var playerData = loadGame.Execute(profileId);
            if (playerData == null)
            {
                errorMessage = "Perfil no encontrado.";
                Debug.Log($"❌ {errorMessage}");
                return false;
            }

            Debug.Log($"🔧 Perfil cargado: {playerData.ProfileName}");
            Debug.Log($"🔧 Contraseña actual existe: {!string.IsNullOrEmpty(playerData.CurrentPasswordHash)}");

            // Verificar contraseña actual si existe
            if (!string.IsNullOrEmpty(playerData.CurrentPasswordHash))
            {
                if (!PasswordHelper.VerifyPassword(currentPassword, playerData.CurrentPasswordHash))
                {
                    errorMessage = "La contraseña actual es incorrecta.";
                    Debug.Log($"❌ {errorMessage}");
                    return false;
                }
                Debug.Log("✅ Contraseña actual verificada");
            }

            // Hashear nueva contraseña
            string newPasswordHash = PasswordHelper.HashPassword(newPassword);
            Debug.Log($"🔧 Nueva contraseña hasheada: {newPasswordHash.Substring(0, 10)}...");

            // Cambiar contraseña
            if (!playerData.ChangePassword(newPasswordHash, out errorMessage))
            {
                Debug.Log($"❌ Error en ChangePassword del dominio: {errorMessage}");
                return false;
            }

            Debug.Log($"✅ Contraseña cambiada en el dominio");
            Debug.Log($"🔧 Historial de contraseñas: {playerData.PasswordHistory?.Count ?? 0} entradas");

            // Guardar cambios
            Debug.Log("🔧 Guardando cambios en la base de datos...");
            saveGame.Execute(playerData);
            Debug.Log($"✅ Contraseña cambiada exitosamente para perfil {profileId}");
            
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.Log($"❌ Error cambiando contraseña: {ex.Message}");
            Debug.Log($"❌ StackTrace: {ex.StackTrace}");
            errorMessage = "Error interno del sistema.";
            return false;
        }
    }

    /// <summary>
    /// Establece una contraseña inicial para un perfil.
    /// </summary>
    public bool SetInitialPassword(int profileId, string password, out string errorMessage)
    {
        errorMessage = "";

        try
        {
            Debug.Log($"🔧 SetInitialPassword iniciado - ProfileId: {profileId}");
            
            if (!PasswordHelper.IsValidPassword(password, out errorMessage))
            {
                Debug.Log($"❌ Contraseña inválida: {errorMessage}");
                return false;
            }

            var playerData = loadGame.Execute(profileId);
            if (playerData == null)
            {
                errorMessage = "Perfil no encontrado.";
                Debug.Log($"❌ {errorMessage}");
                return false;
            }

            Debug.Log($"🔧 Perfil cargado: {playerData.ProfileName}");

            // Solo permitir si no tiene contraseña
            if (!string.IsNullOrEmpty(playerData.CurrentPasswordHash))
            {
                errorMessage = "Este perfil ya tiene una contraseña establecida.";
                Debug.LogWarning($"⚠️ {errorMessage}");
                return false;
            }

            string passwordHash = PasswordHelper.HashPassword(password);
            Debug.Log($"🔧 Contraseña inicial hasheada: {passwordHash.Substring(0, 10)}...");

            playerData.CurrentPasswordHash = passwordHash;
            
            if (playerData.PasswordHistory == null)
                playerData.PasswordHistory = new System.Collections.Generic.List<string>();
            
            playerData.PasswordHistory.Add(passwordHash);
            playerData.LastPasswordChange = System.DateTime.Now;

            Debug.Log("🔧 Guardando contraseña inicial en la base de datos...");
            saveGame.Execute(playerData);
            Debug.Log($"✅ Contraseña inicial establecida para perfil {profileId}");
            
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.Log($"❌ Error estableciendo contraseña inicial: {ex.Message}");
            Debug.Log($"❌ StackTrace: {ex.StackTrace}");
            errorMessage = "Error interno del sistema.";
            return false;
        }
    }

    /// <summary>
    /// Verifica si un perfil tiene contraseña establecida.
    /// </summary>
    public bool HasPassword(int profileId)
    {
        try
        {
            var playerData = loadGame.Execute(profileId);
            return playerData != null && !string.IsNullOrEmpty(playerData.CurrentPasswordHash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Verifica una contraseña para un perfil.
    /// </summary>
    public bool VerifyPassword(int profileId, string password)
    {
        try
        {
            var playerData = loadGame.Execute(profileId);
            if (playerData == null || string.IsNullOrEmpty(playerData.CurrentPasswordHash))
                return false;

            return PasswordHelper.VerifyPassword(password, playerData.CurrentPasswordHash);
        }
        catch
        {
            return false;
        }
    }
}