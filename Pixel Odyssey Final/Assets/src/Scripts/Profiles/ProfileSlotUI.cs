using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProfileSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int profileId;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text sceneText;
    public Image infoImage;

    // Arrastra aquí las imágenes de las esquinas en el inspector
    public GameObject cornerTopLeft;
    public GameObject cornerTopRight;
    public GameObject cornerBottomLeft;
    public GameObject cornerBottomRight;

    private void Start()
    {
        Debug.Log($"[ProfileSlotUI] Start() para perfil {profileId}");
        UpdateProfileInfo();
        SetCornersActive(false);
    }

    public void UpdateProfileInfo()
    {
        Debug.Log($"[ProfileSlotUI] UpdateProfileInfo() para perfil {profileId}");
        var persistence = FindObjectOfType<PersistenceController>();
        bool hasData = false;
        if (persistence != null)
        {
            var data = persistence.GetProfileData(profileId);
            Debug.Log($"[ProfileSlotUI] Datos cargados para perfil {profileId}: {(data != null ? JsonUtility.ToJson(data) : "null")}");
            nameText.text = persistence.GetProfileName(profileId);
            healthText.text = "Salud:" + persistence.GetProfileHealth(profileId);
            sceneText.text = persistence.GetProfileScene(profileId);
            hasData = persistence.GetProfileScene(profileId) != "Sin partida";
        }
        else
        {
            Debug.LogWarning($"[ProfileSlotUI] No se encontró PersistenceController en la escena para perfil {profileId}");
            nameText.text = $"Perfil {profileId}";
            healthText.text = "-";
            sceneText.text = "Escena: -";
        }
        if (infoImage != null)
        {
            infoImage.gameObject.SetActive(hasData);
            Debug.Log($"[ProfileSlotUI] infoImage {(hasData ? "activada" : "desactivada")} para perfil {profileId}");
        }
    }

    // Métodos para el efecto hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetCornersActive(true);
        Debug.Log($"[ProfileSlotUI] Hover ON en perfil {profileId}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetCornersActive(false);
        Debug.Log($"[ProfileSlotUI] Hover OFF en perfil {profileId}");
    }

    private void SetCornersActive(bool active)
    {
        if (cornerTopLeft != null) cornerTopLeft.SetActive(active);
        if (cornerTopRight != null) cornerTopRight.SetActive(active);
        if (cornerBottomLeft != null) cornerBottomLeft.SetActive(active);
        if (cornerBottomRight != null) cornerBottomRight.SetActive(active);
    }
  
}