using UnityEngine;

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
