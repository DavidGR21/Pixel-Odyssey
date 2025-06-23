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
        UpdateProfileInfo();
        SetCornersActive(false);
    }

    public void UpdateProfileInfo()
    {
        var persistence = FindObjectOfType<PersistenceController>();
        bool hasData = false;
        if (persistence != null)
        {
            nameText.text = persistence.GetProfileName(profileId);
            healthText.text = "Salud: " + persistence.GetProfileHealth(profileId);
            sceneText.text = persistence.GetProfileScene(profileId);
            hasData = persistence.GetProfileScene(profileId) != "Sin partida";
        }
        else
        {
            nameText.text = $"Perfil {profileId}";
            healthText.text = "Vida: -";
            sceneText.text = "Escena: -";
        }
        if (infoImage != null)
            infoImage.gameObject.SetActive(hasData);
    }

    // Métodos para el efecto hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetCornersActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetCornersActive(false);
    }

    private void SetCornersActive(bool active)
    {
        if (cornerTopLeft != null) cornerTopLeft.SetActive(active);
        if (cornerTopRight != null) cornerTopRight.SetActive(active);
        if (cornerBottomLeft != null) cornerBottomLeft.SetActive(active);
        if (cornerBottomRight != null) cornerBottomRight.SetActive(active);
    }
}