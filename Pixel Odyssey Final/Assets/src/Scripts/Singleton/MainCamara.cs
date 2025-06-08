using UnityEngine;

public class MainCamara : MonoBehaviour
{
    private static MainCamara instance;

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
