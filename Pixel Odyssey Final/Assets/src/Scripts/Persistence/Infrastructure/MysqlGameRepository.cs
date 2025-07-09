/*
using System;
using MySql.Data.MySqlClient;
using UnityEngine;

/// <summary>
/// Implementación del repositorio de juegos utilizando MySQL.
/// Esta clase se encarga de guardar y cargar los datos del jugador en una base de datos MySQL.
/// Implementa la interfaz IGameRepository manteniendo los principios SOLID.
/// </summary>
public class MySQLGameRepository : IGameRepository
{
    private readonly string connectionString;

    public MySQLGameRepository(string connectionString)
    {
        this.connectionString = connectionString;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS PlayerProfiles (
                        ProfileId INT PRIMARY KEY,
                        ProfileName VARCHAR(255) NOT NULL,
                        Health FLOAT NOT NULL,
                        Shield FLOAT NOT NULL,
                        SpawnPointName VARCHAR(255),
                        CurrentScene VARCHAR(255),
                        PositionX FLOAT NOT NULL,
                        PositionY FLOAT NOT NULL,
                        PositionZ FLOAT NOT NULL,
                        CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                    )";
                
                using (var command = new MySqlCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error inicializando base de datos: {ex.Message}");
        }
    }

    public void Save(PlayerData data)
    {
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    INSERT INTO PlayerProfiles 
                    (ProfileId, ProfileName, Health, Shield, SpawnPointName, CurrentScene, PositionX, PositionY, PositionZ)
                    VALUES (@ProfileId, @ProfileName, @Health, @Shield, @SpawnPointName, @CurrentScene, @PositionX, @PositionY, @PositionZ)
                    ON DUPLICATE KEY UPDATE
                    ProfileName = VALUES(ProfileName),
                    Health = VALUES(Health),
                    Shield = VALUES(Shield),
                    SpawnPointName = VALUES(SpawnPointName),
                    CurrentScene = VALUES(CurrentScene),
                    PositionX = VALUES(PositionX),
                    PositionY = VALUES(PositionY),
                    PositionZ = VALUES(PositionZ),
                    UpdatedAt = CURRENT_TIMESTAMP";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProfileId", data.ProfileId);
                    command.Parameters.AddWithValue("@ProfileName", data.ProfileName ?? "");
                    command.Parameters.AddWithValue("@Health", data.Health);
                    command.Parameters.AddWithValue("@Shield", data.Shield);
                    command.Parameters.AddWithValue("@SpawnPointName", data.SpawnPointName ?? "");
                    command.Parameters.AddWithValue("@CurrentScene", data.CurrentScene ?? "");
                    command.Parameters.AddWithValue("@PositionX", data.PositionX);
                    command.Parameters.AddWithValue("@PositionY", data.PositionY);
                    command.Parameters.AddWithValue("@PositionZ", data.PositionZ);
                    
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error guardando datos en MySQL: {ex.Message}");
            throw;
        }
    }

    public PlayerData Load(int profileId)
    {
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM PlayerProfiles WHERE ProfileId = @ProfileId";
                
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProfileId", profileId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PlayerData
                            {
                                ProfileId = reader["ProfileId"] as int? ?? 0,
                                ProfileName = reader["ProfileName"] as string ?? "",
                                Health = Convert.ToSingle(reader["Health"]),
                                Shield = Convert.ToSingle(reader["Shield"]),
                                SpawnPointName = reader["SpawnPointName"] as string,
                                CurrentScene = reader["CurrentScene"] as string,
                                PositionX = Convert.ToSingle(reader["PositionX"]),
                                PositionY = Convert.ToSingle(reader["PositionY"]),
                                PositionZ = Convert.ToSingle(reader["PositionZ"])
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error cargando datos desde MySQL: {ex.Message}");
        }
        
        return null;
    }
}
*/