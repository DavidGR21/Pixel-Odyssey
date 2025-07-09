using UnityEngine;
/// <summary>
/// Clase encargada de configurar la escena de créditos.
/// </summary>
public class CreditsSceneSetup : MonoBehaviour
{
    void Start()
    {
        GameObject hud = GameObject.Find("HudPlayer");

        if (hud != null)
        {
            hud.SetActive(false);
        }
     
    }
}
