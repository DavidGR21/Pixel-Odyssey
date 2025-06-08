using UnityEngine;

public class VirtualCamara : MonoBehaviour
{
    private static VirtualCamara instance;

    void Awake()
    {
        if (instance == null)
        {
            // Este es el primer objeto, se mantiene
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Ya existe una instancia, se destruye este objeto nuevo
            Destroy(gameObject);
        }
    }
}
