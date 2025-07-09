using System;
using System.Collections.Generic;

/// <summary>
/// Clase que almacena los datos del jugador, incluyendo información de usuario y contraseñas.
/// Contiene toda la información necesaria para la persistencia del perfil del jugador.
/// </summary>
[System.Serializable]
public class PlayerData
{
    // Profile Info
    public int ProfileId;
    public string ProfileName;
    
    // User Authentication
    public string Username;
    public string Email;
    public string CurrentPasswordHash;
    public List<string> PasswordHistory;
    public DateTime LastPasswordChange;
    
    // Game Stats
    public float Health;
    public float Shield;
    public string SpawnPointName;
    public string CurrentScene;
    public float PositionX;
    public float PositionY;
    public float PositionZ;
    
    // Timestamps
    public DateTime CreatedAt;
    public DateTime LastUpdated;

    public PlayerData()
    {
        PasswordHistory = new List<string>();
        CreatedAt = DateTime.Now;
        LastUpdated = DateTime.Now;
        LastPasswordChange = DateTime.Now;
    }

    /// <summary>
    /// Verifica si una contraseña ya fue utilizada anteriormente.
    /// </summary>
    public bool IsPasswordUsedBefore(string passwordHash)
    {
        return PasswordHistory != null && PasswordHistory.Contains(passwordHash);
    }

    /// <summary>
    /// Cambia la contraseña del usuario verificando que no sea una anterior.
    /// </summary>
    public bool ChangePassword(string newPasswordHash, out string errorMessage)
    {
        errorMessage = "";

        if (string.IsNullOrEmpty(newPasswordHash))
        {
            errorMessage = "La contraseña no puede estar vacía.";
            return false;
        }

        if (IsPasswordUsedBefore(newPasswordHash))
        {
            errorMessage = "No puedes usar una contraseña que ya has utilizado anteriormente.";
            return false;
        }

        // Inicializar lista si es null
        if (PasswordHistory == null)
            PasswordHistory = new List<string>();

        // Agregar contraseña actual al historial si no está
        if (!string.IsNullOrEmpty(CurrentPasswordHash) && !PasswordHistory.Contains(CurrentPasswordHash))
        {
            PasswordHistory.Add(CurrentPasswordHash);
        }

        // Actualizar contraseña
        CurrentPasswordHash = newPasswordHash;
        PasswordHistory.Add(newPasswordHash);
        LastPasswordChange = DateTime.Now;
        LastUpdated = DateTime.Now;

        return true;
    }

    /// <summary>
    /// Verifica si la contraseña actual coincide con el hash proporcionado.
    /// </summary>
    public bool VerifyPassword(string passwordHash)
    {
        return CurrentPasswordHash == passwordHash;
    }
}