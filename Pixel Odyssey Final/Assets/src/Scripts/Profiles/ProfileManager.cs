using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Clase encargada de gestionar los perfiles del jugador.
/// Permite crear, cargar, eliminar y seleccionar perfiles.
/// Utiliza un sistema de persistencia para guardar los datos de los perfiles en archivos JSON.
/// También actualiza la interfaz de usuario con la información de los perfiles disponibles.
/// </summary>
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadAllProfiles();
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
        if (persistence == null)
        {
            Debug.LogError("[ProfileManager] PersistenceController no encontrado en LoadAllProfiles");
            return;
        }
        for (int i = 1; i <= MaxProfiles; i++)
        {
            var data = persistence.GetProfileData(i);
            Profiles.Add(data);
        }
    }
    public void UpdateAllProfileSlots()
    {
        var slots = FindObjectsOfType<ProfileSlotUI>();
        foreach (var slot in slots)
        {
            slot.UpdateProfileInfo();
        }
    }
}