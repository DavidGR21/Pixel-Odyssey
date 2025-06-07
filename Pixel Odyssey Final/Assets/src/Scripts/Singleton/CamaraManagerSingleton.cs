using UnityEngine;

public class CamaraManagerSingleton : MonoBehaviour
{
    private static CamaraManagerSingleton instance;

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
