using System.IO;
using UnityEngine;
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
}

