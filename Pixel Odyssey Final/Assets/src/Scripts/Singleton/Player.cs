using UnityEngine;
/// <summary>
/// Clase encargada de gestionar al jugador como un singleton.
/// </summary>
public class Player : MonoBehaviour
{
    private static Player instance;

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