using UnityEngine;

public class AudioManager : MonoBehaviour 

{
    [SerializeField] private AudioSource sfxAudioSource, musicAudioSource;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMusic();
        }
    }
    public void PlaySound(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);   

    }
    private void ToggleMusic()
    {
        musicAudioSource.mute=!musicAudioSource.mute;
  
    }
    }



