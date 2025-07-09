using UnityEngine;
/// <summary>
/// Clase encargada de gestionar la música de la escena actual.
/// Reproduce la música de fondo al inicio de la escena y permite configurarla para que se repita.
/// Esta clase debe ser añadida a un GameObject en la escena para que funcione correctamente.
/// </summary>
public class SceneMusicController : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;
    [SerializeField] private bool loop = true;

    private void Start()
    {
        if (sceneMusic != null)
        {
            AudioManager.Instance.SetMusicVolume(2);
            AudioManager.Instance.PlayMusic(sceneMusic, loop);
        }
    }
}
