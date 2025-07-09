using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Clase que implementa un repositorio de juegos utilizando archivos.
/// Esta clase se encarga de guardar y cargar los datos del jugador en archivos JSON.
/// </summary>
public class FileGameRepository : IGameRepository
{
    private string GetPath(int profileId)
    {
        return Application.persistentDataPath + $"/save_profile_{profileId}.json";
    }

    // Este método guarda los datos del jugador en un archivo JSON basado en el ProfileId
    public void Save(PlayerData data)
    {
        // Guarda usando el ProfileId del PlayerData
        File.WriteAllText(GetPath(data.ProfileId), JsonUtility.ToJson(data));
    }

    //Este metodo carga los datos del jugador desde un archivo JSON basado en el ProfileId
    public PlayerData Load(int profileId)
    {
        string path = GetPath(profileId);
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<PlayerData>(File.ReadAllText(path));
    }

    public bool DeleteProfile(int profileId)
    {
        string path = GetPath(profileId);
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }

    public PlayerData[] LoadAllProfiles()
    {
        var profiles = new List<PlayerData>();
        var files = Directory.GetFiles(Application.persistentDataPath, "save_profile_*.json");

        foreach (var file in files)
        {
            try
            {
                var data = JsonUtility.FromJson<PlayerData>(File.ReadAllText(file));
                if (data != null) profiles.Add(data);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error cargando perfil desde {file}: {ex.Message}");
            }
        }

        return profiles.ToArray();
    }
}

