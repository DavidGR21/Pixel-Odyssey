using UnityEngine;
/// <summary>
/// Clase encargada de gestionar el canvas del juego como un singleton.
/// </summary>
public class Canva : MonoBehaviour
{
    private static Canva instance;

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
