using UnityEngine;

/// <summary>
/// Configuración de la base de datos.
/// Ahora soporta SQLite (local) y puede expandirse para otros tipos de base de datos.
/// Permite configurar diferentes entornos y opciones de base de datos.
/// </summary>
[CreateAssetMenu(fileName = "DatabaseConfig", menuName = "Config/Database Config")]
public class DatabaseConfig : ScriptableObject
{
    [Header("Database Type")]
    public DatabaseType databaseType = DatabaseType.SQLite;
    
    [Header("SQLite Configuration")]
    public string databaseName = "PixelOdyssey.db";
    
    [Header("MySQL Configuration (para uso futuro)")]
    public string server = "localhost";
    public string database = "pixel_odyssey";
    public string uid = "root";
    public string password = "";
    public int port = 3306;
    
    [Header("Connection Settings")]
    public int connectionTimeout = 30;
    public bool enableLogging = true;

    public enum DatabaseType
    {
        SQLite,
        MySQL,
        File
    }

    public string GetSQLiteConnectionString()
    {
        var databasePath = System.IO.Path.Combine(Application.persistentDataPath, databaseName);
        return $"URI=file:{databasePath}";
    }

    public string GetMySQLConnectionString()
    {
        return $"Server={server};Database={database};Uid={uid};Pwd={password};Port={port};" +
               $"Connection Timeout={connectionTimeout};";
    }
}