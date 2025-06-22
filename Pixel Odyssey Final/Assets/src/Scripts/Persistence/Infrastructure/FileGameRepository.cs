using System.IO;
using UnityEngine;

public class FileGameRepository : IGameRepository
{
    private string path = Application.persistentDataPath + "/save.json";
    public void Save(PlayerData data)
    {
        File.WriteAllText(path, JsonUtility.ToJson(data));
    }
    public PlayerData Load()
    {
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<PlayerData>(File.ReadAllText(path));
    }
}

