using UnityEngine;
/// <summary>
/// Clase encargada de gestionar la cámara virtual del juego como un singleton.
/// </summary>
public class VirtualCamara : MonoBehaviour
{
    private static VirtualCamara instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
