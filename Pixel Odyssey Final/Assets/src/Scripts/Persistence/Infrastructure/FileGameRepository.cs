using System.IO;
using UnityEngine;

public class FileGameRepository : IGameRepository
{
    private string GetPath(int profileId)
    {
        return Application.persistentDataPath + $"/save_profile_{profileId}.json";
    }

    public void Save(PlayerData data)
    {
        // Guarda usando el ProfileId del PlayerData
        File.WriteAllText(GetPath(data.ProfileId), JsonUtility.ToJson(data));
    }

    public PlayerData Load()
    {
        // Carga el perfil por defecto (puedes dejarlo así o eliminarlo si solo usas perfiles)
        string path = Application.persistentDataPath + "/save.json";
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<PlayerData>(File.ReadAllText(path));
    }

    public PlayerData Load(int profileId)
    {
        string path = GetPath(profileId);
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<PlayerData>(File.ReadAllText(path));
    }
}

