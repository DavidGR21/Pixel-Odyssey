using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SQLite4Unity3d;

/// <summary>
/// Implementación REAL del repositorio de juegos utilizando SQLite4Unity3d.
/// Esta implementación usa SQLite verdadero con consultas SQL reales.
/// Implementa la interfaz IGameRepository manteniendo los principios SOLID.
/// </summary>
public class SQLiteGameRepository : IGameRepository
{
    private readonly string databasePath;
    private SQLiteConnection connection;

    public SQLiteGameRepository(string databaseName = "PixelOdyssey.db")
    {
        databasePath = Path.Combine(Application.persistentDataPath, databaseName);
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            // Crear conexión SQLite real
            connection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            
            // Verificar si la tabla existe y tiene la columna PlayerDataJson
            try
            {
                var tableInfo = connection.GetTableInfo("PlayerProfiles");
                bool hasPlayerDataJson = tableInfo.Any(column => column.Name == "PlayerDataJson");
                
                if (!hasPlayerDataJson)
                {
                    Debug.Log("🔧 Tabla antigua detectada, recreando con nuevos campos...");
                    connection.DropTable<PlayerProfileEntity>();
                }
            }
            catch
            {
                // La tabla no existe, se creará automáticamente
                Debug.Log("🔧 Tabla PlayerProfiles no existe, será creada");
            }
            
            // Crear tabla si no existe
            connection.CreateTable<PlayerProfileEntity>();
            
            Debug.Log($"✅ Base de datos SQLite REAL inicializada en: {databasePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error inicializando base de datos SQLite: {ex.Message}");
        }
    }

    public void Save(PlayerData data)
    {
        try
        {
            Debug.Log($"🔧 SQLiteGameRepository.Save iniciado para ProfileId: {data.ProfileId}");
            
            // Verificar que la conexión esté activa
            if (connection == null)
            {
                InitializeDatabase();
            }

            // Convertir PlayerData a JSON para guardar todos los campos
            string playerDataJson = JsonUtility.ToJson(data);
            Debug.Log($"🔧 PlayerData serializado: {playerDataJson}");

            var entity = new PlayerProfileEntity
            {
                ProfileId = data.ProfileId,
                ProfileName = data.ProfileName ?? $"Perfil {data.ProfileId}",
                Health = data.Health,
                Shield = data.Shield,
                SpawnPointName = data.SpawnPointName,
                CurrentScene = data.CurrentScene,
                PositionX = data.PositionX,
                PositionY = data.PositionY,
                PositionZ = data.PositionZ,
                PlayerDataJson = playerDataJson, // ← Campo que almacena TODOS los datos
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            Debug.Log($"🔧 Entity creada - PasswordHash existe: {!string.IsNullOrEmpty(data.CurrentPasswordHash)}");
            Debug.Log($"🔧 PasswordHistory count: {data.PasswordHistory?.Count ?? 0}");

            // Usar InsertOrReplace para actualizar o insertar
            int result = connection.InsertOrReplace(entity);
            Debug.Log($"✅ Datos guardados en SQLite para ProfileId: {data.ProfileId}, Resultado: {result}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error guardando datos en SQLite: {ex.Message}");
            Debug.LogError($"❌ StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public PlayerData Load(int profileId)
    {
        try
        {
            Debug.Log($"🔧 SQLiteGameRepository.Load iniciado para ProfileId: {profileId}");
            
            var entity = connection.Table<PlayerProfileEntity>()
                                  .Where(p => p.ProfileId == profileId)
                                  .FirstOrDefault();
            
            if (entity != null)
            {
                Debug.Log($"🔧 Entity encontrada para ProfileId: {profileId}");
                
                // Intentar deserializar desde PlayerDataJson primero (NUEVO MÉTODO)
                if (!string.IsNullOrEmpty(entity.PlayerDataJson))
                {
                    try
                    {
                        var playerData = JsonUtility.FromJson<PlayerData>(entity.PlayerDataJson);
                        Debug.Log($"✅ Datos cargados desde JSON para ProfileId: {profileId}");
                        Debug.Log($"🔧 Contraseña existe en JSON: {!string.IsNullOrEmpty(playerData.CurrentPasswordHash)}");
                        Debug.Log($"🔧 PasswordHistory count desde JSON: {playerData.PasswordHistory?.Count ?? 0}");
                        return playerData;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"⚠️ Error deserializando JSON, usando campos individuales: {ex.Message}");
                    }
                }
                
                // Fallback: usar campos individuales (para compatibilidad con datos antiguos)
                Debug.Log("🔧 Usando campos individuales (fallback para datos antiguos)");
                return new PlayerData
                {
                    ProfileId = entity.ProfileId,
                    ProfileName = entity.ProfileName,
                    Health = entity.Health,
                    Shield = entity.Shield,
                    SpawnPointName = entity.SpawnPointName,
                    CurrentScene = entity.CurrentScene,
                    PositionX = entity.PositionX,
                    PositionY = entity.PositionY,
                    PositionZ = entity.PositionZ,
                    // Los campos de contraseña estarán vacíos en datos antiguos
                    CurrentPasswordHash = "",
                    PasswordHistory = new System.Collections.Generic.List<string>(),
                    LastPasswordChange = System.DateTime.Now,
                    CreatedAt = System.DateTime.Now,
                    LastUpdated = System.DateTime.Now
                };
            }
            
            Debug.Log($"⚠️ No se encontraron datos para ProfileId: {profileId}");
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error cargando datos desde SQLite: {ex.Message}");
            return null;
        }
    }

    public bool DeleteProfile(int profileId)
    {
        try
        {
            int rowsAffected = connection.Delete<PlayerProfileEntity>(profileId);
            Debug.Log($"Perfil {profileId} eliminado. Filas afectadas: {rowsAffected}");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error eliminando perfil desde SQLite: {ex.Message}");
            return false;
        }
    }

    public PlayerData[] LoadAllProfiles()
    {
        try
        {
            Debug.Log("🔧 LoadAllProfiles iniciado");
            
            var entities = connection.Table<PlayerProfileEntity>()
                                   .OrderByDescending(p => p.UpdatedAt)
                                   .ToList();
            
            var profiles = new List<PlayerData>();
            
            foreach (var entity in entities)
            {
                // Intentar deserializar desde PlayerDataJson primero
                if (!string.IsNullOrEmpty(entity.PlayerDataJson))
                {
                    try
                    {
                        var playerData = JsonUtility.FromJson<PlayerData>(entity.PlayerDataJson);
                        profiles.Add(playerData);
                        continue;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"⚠️ Error deserializando JSON para ProfileId {entity.ProfileId}: {ex.Message}");
                    }
                }
                
                // Fallback: usar campos individuales
                profiles.Add(new PlayerData
                {
                    ProfileId = entity.ProfileId,
                    ProfileName = entity.ProfileName,
                    Health = entity.Health,
                    Shield = entity.Shield,
                    SpawnPointName = entity.SpawnPointName,
                    CurrentScene = entity.CurrentScene,
                    PositionX = entity.PositionX,
                    PositionY = entity.PositionY,
                    PositionZ = entity.PositionZ,
                    CurrentPasswordHash = "",
                    PasswordHistory = new System.Collections.Generic.List<string>(),
                    LastPasswordChange = System.DateTime.Now,
                    CreatedAt = System.DateTime.Now,
                    LastUpdated = System.DateTime.Now
                });
            }
            
            Debug.Log($"✅ Cargados {profiles.Count} perfiles desde SQLite");
            return profiles.ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error cargando todos los perfiles desde SQLite: {ex.Message}");
            return new PlayerData[0];
        }
    }

    // Método para verificar conexión
    public bool IsConnected()
    {
        try
        {
            return connection != null && connection.TableMappings.Any();
        }
        catch
        {
            return false;
        }
    }

    // Método para cerrar conexión
    public void CloseConnection()
    {
        try
        {
            connection?.Close();
            connection?.Dispose();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error cerrando conexión SQLite: {ex.Message}");
        }
    }

    // Método de debug para verificar datos
    public void DebugTableContents()
    {
        try
        {
            Debug.Log("🔍 === CONTENIDO DE LA TABLA ===");
            var entities = connection.Table<PlayerProfileEntity>().ToList();
            
            foreach (var entity in entities)
            {
                Debug.Log($"ProfileId: {entity.ProfileId}");
                Debug.Log($"  - Name: {entity.ProfileName}");
                Debug.Log($"  - Health: {entity.Health}");
                Debug.Log($"  - HasJson: {!string.IsNullOrEmpty(entity.PlayerDataJson)}");
                
                if (!string.IsNullOrEmpty(entity.PlayerDataJson))
                {
                    try
                    {
                        var data = JsonUtility.FromJson<PlayerData>(entity.PlayerDataJson);
                        Debug.Log($"  - Password exists: {!string.IsNullOrEmpty(data.CurrentPasswordHash)}");
                        Debug.Log($"  - Password history: {data.PasswordHistory?.Count ?? 0}");
                    }
                    catch
                    {
                        Debug.Log($"  - JSON invalid");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en debug: {ex.Message}");
        }
    }
}

/// <summary>
/// Entidad que representa la tabla PlayerProfiles en SQLite.
/// Usa atributos de SQLite4Unity3d para mapear a la base de datos.
/// ACTUALIZADA con el campo PlayerDataJson para guardar datos completos.
/// </summary>
[Table("PlayerProfiles")]
public class PlayerProfileEntity
{
    [PrimaryKey]
    public int ProfileId { get; set; }
    
    [MaxLength(255)]
    public string ProfileName { get; set; }
    
    public float Health { get; set; }
    public float Shield { get; set; }
    
    [MaxLength(255)]
    public string SpawnPointName { get; set; }
    
    [MaxLength(255)]
    public string CurrentScene { get; set; }
    
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
    
    [MaxLength(50)]
    public string UpdatedAt { get; set; }
    
    /// <summary>
    /// Nuevo campo que almacena TODOS los datos de PlayerData como JSON.
    /// Esto incluye campos de contraseña, historial, fechas, etc.
    /// </summary>
    public string PlayerDataJson { get; set; }
}