using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private AudioClip startSound;

    public void PlayGame()
    {
        Debug.Log("Cargando escena de selección de perfil...");
        SceneManager.LoadScene("Profiles", LoadSceneMode.Single); // Cambia "Profiles" por el nombre exacto de tu escena de perfiles
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    /*
    private IEnumerator PlayAndLoadWithBootstrap(string bootstrapScene, string targetScene)
    {
        AudioManager.Instance.PlaySound(startSound);
        yield return new WaitForSeconds(startSound.length);
        Debug.Log("Iniciando carga de escena...");
        Debug.Log("Escena objetivo: " + targetScene);
        Debug.Log(bootstrapScene == targetScene);

        if (bootstrapScene == targetScene)
        {
            Debug.Log("La escena guardada es la bootstrap, cargando solo una vez.");
            SceneManager.LoadScene(bootstrapScene, LoadSceneMode.Single);
            yield break;
        }
        Debug.Log(bootstrapScene + " " + LoadSceneMode.Single);

        // 1. Carga la escena bootstrap primero
        var op1 = SceneManager.LoadSceneAsync(bootstrapScene, LoadSceneMode.Single);
        yield return op1;
        Debug.Log("Escena bootstrap cargada: " + bootstrapScene);

        // 2. Espera un frame para que los objetos persistentes se inicialicen
        yield return null;

        Debug.Log("Frame de espera tras bootstrap.");

        // 3. Carga la escena guardada
        var op2 = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        yield return op2;
        Debug.Log("Escena guardada cargada: " + targetScene);

        // 4. Espera un frame para asegurarte de que el jugador ya está en la escena
        yield return null;

        // 5. Ahora sí, posiciona al jugador
        var persistence = FindObjectOfType<PersistenceController>();
        Debug.Log("persistence =" + (persistence != null));
        if (persistence != null)
        {
            Debug.Log("Cargando datos del juego...");
            var data = persistence.LoadGameData();
            if (data != null)
            {
                Debug.Log("Datos del juego cargados: " + data.CurrentScene);
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Debug.Log(player != null ? "Jugador encontrado." : "Jugador no encontrado.");
                if (player != null)
                {
                    player.transform.position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
                    Debug.Log("Posición del jugador restaurada: " + player.transform.position);

                    // Restaurar salud y escudo
                    var healthPlayer = player.GetComponent<HealthPlayer>();
                    if (healthPlayer != null)
                    {
                        healthPlayer.RestoreHealthAndShield(data.Health, data.Shield);


                        Debug.Log("Salud y escudo restaurados: " + data.Health + " / " + data.Shield);
                    }
                    else
                    {
                        Debug.LogWarning("No se encontró el componente HealthPlayer en el jugador.");
                    }
                }
                else
                {
                    Debug.LogWarning("No se encontró el jugador para posicionar.");
                }
            }
        }

        Destroy(gameObject);
    }
    */
    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
