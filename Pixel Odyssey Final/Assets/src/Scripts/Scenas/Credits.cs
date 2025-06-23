using UnityEngine;

public class CreditsSceneSetup : MonoBehaviour
{
    void Start()
    {
        GameObject hud = GameObject.Find("HudPlayer");

        if (hud != null)
        {
            hud.SetActive(false);
            Debug.Log("HUDPlayer desactivado en la escena de créditos.");
        }
        else
        {
            Debug.LogWarning("HUDPlayer no encontrado en la escena de créditos.");
        }
    }
}
