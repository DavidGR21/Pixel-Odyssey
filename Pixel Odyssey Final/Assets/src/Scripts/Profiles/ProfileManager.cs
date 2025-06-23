using UnityEngine;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    public int MaxProfiles = 3;
    public List<PlayerData> Profiles = new List<PlayerData>();
    public int ActiveProfileId { get; private set; } = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllProfiles();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetActiveProfile(int id)
    {
        ActiveProfileId = id;
    }

    public PlayerData GetProfile(int id)
    {
        var persistence = FindObjectOfType<PersistenceController>();
        return persistence != null ? persistence.GetProfileData(id) : null;
    }

    public void DeleteProfile(int id)
    {
        string path = Application.persistentDataPath + $"/save_profile_{id}.json";
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
    }

    private void LoadAllProfiles()
    {
        Profiles.Clear();
        var persistence = FindObjectOfType<PersistenceController>();
        for (int i = 1; i <= MaxProfiles; i++)
        {
            var data = persistence != null ? persistence.GetProfileData(i) : null;
            Profiles.Add(data);
        }
    }
}