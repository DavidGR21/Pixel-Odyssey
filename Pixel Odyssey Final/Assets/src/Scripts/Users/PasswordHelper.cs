using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Helper simple para el manejo de contraseñas.
/// Proporciona funciones básicas de hash y verificación.
/// </summary>
public static class PasswordHelper
{
    private const string SALT = "PIXEL_ODYSSEY_SALT_2024";

    /// <summary>
    /// Crea un hash de la contraseña usando SHA256.
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return "";

        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password + SALT));
            return Convert.ToBase64String(bytes);
        }
    }

    /// <summary>
    /// Verifica si una contraseña coincide con el hash.
    /// </summary>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        string hashOfInput = HashPassword(password);
        return hashOfInput == hashedPassword;
    }

    /// <summary>
    /// Valida que una contraseña cumpla con los requisitos mínimos.
    /// </summary>
    public static bool IsValidPassword(string password, out string errorMessage)
    {
        errorMessage = "";

        if (string.IsNullOrEmpty(password))
        {
            errorMessage = "La contraseña no puede estar vacía.";
            return false;
        }

        if (password.Length < 6)
        {
            errorMessage = "La contraseña debe tener al menos 6 caracteres.";
            return false;
        }

        return true;
    }
}