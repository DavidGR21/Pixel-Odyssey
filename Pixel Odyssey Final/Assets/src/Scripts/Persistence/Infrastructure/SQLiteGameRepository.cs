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
            
            // Crear tabla si no existe
            connection.CreateTable<PlayerProfileEntity>();
            
            Debug.Log($"Base de datos SQLite REAL inicializada en: {databasePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error inicializando base de datos SQLite: {ex.Message}");
        }
    }

    public void Save(PlayerData data)
    {
        try
        {
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
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Usar InsertOrReplace para actualizar o insertar
            int result = connection.InsertOrReplace(entity);
            Debug.Log($"Datos guardados en SQLite para ProfileId: {data.ProfileId}, Resultado: {result}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error guardando datos en SQLite: {ex.Message}");
            throw;
        }
    }

    public PlayerData Load(int profileId)
    {
        try
        {
            var entity = connection.Table<PlayerProfileEntity>()
                                  .Where(p => p.ProfileId == profileId)
                                  .FirstOrDefault();
            
            if (entity != null)
            {
                Debug.Log($"Datos cargados desde SQLite para ProfileId: {profileId}");
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
                    PositionZ = entity.PositionZ
                };
            }
            
            Debug.Log($"No se encontraron datos para ProfileId: {profileId}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error cargando datos desde SQLite: {ex.Message}");
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
            var entities = connection.Table<PlayerProfileEntity>()
                                   .OrderByDescending(p => p.UpdatedAt)
                                   .ToList();
            
            var profiles = new List<PlayerData>();
            
            foreach (var entity in entities)
            {
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
                    PositionZ = entity.PositionZ
                });
            }
            
            Debug.Log($"Cargados {profiles.Count} perfiles desde SQLite");
            return profiles.ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error cargando todos los perfiles desde SQLite: {ex.Message}");
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
}

/// <summary>
/// Entidad que representa la tabla PlayerProfiles en SQLite.
/// Usa atributos de SQLite4Unity3d para mapear a la base de datos.
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
}