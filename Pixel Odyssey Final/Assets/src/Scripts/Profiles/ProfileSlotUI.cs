using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// Clase encargada de gestionar la interfaz de usuario de un slot de perfil.
/// Muestra la información del perfil, como nombre, salud y escena actual.
/// Permite manejar eventos de hover para mostrar esquinas activas.
/// </summary>
public class ProfileSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int profileId;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text sceneText;
    public Image infoImage;
    public GameObject cornerTopLeft;
    public GameObject cornerTopRight;
    public GameObject cornerBottomLeft;
    public GameObject cornerBottomRight;

    [Header("User Management")]
    public Button changePasswordButton;  // ← Asegúrate de que esta línea esté

    private void Start()
    {
        Debug.Log($"🔧 ProfileSlotUI Start() - ProfileId: {profileId}");
        
        UpdateProfileInfo();
        SetCornersActive(false);
        
        // Debug para el botón de cambio de contraseña
        if (changePasswordButton != null)
        {
            Debug.Log($"✅ ChangePasswordButton encontrado en ProfileSlotUI ProfileId: {profileId}");
            changePasswordButton.onClick.AddListener(OnChangePasswordClicked);
            Debug.Log($"✅ Listener agregado al ChangePasswordButton ProfileId: {profileId}");
        }
        else
        {
            Debug.LogError($"❌ ChangePasswordButton es NULL en ProfileSlotUI ProfileId: {profileId}");
        }
    }

    private void OnChangePasswordClicked()
    {
        Debug.Log($"🔘 ProfileSlotUI: OnChangePasswordClicked ejecutado - ProfileId: {profileId}");
        
        // Buscar incluyendo objetos inactivos
        var changePasswordUI = FindObjectOfType<ChangePasswordUI>(true);
        Debug.Log($"🔍 ChangePasswordUI encontrado (incluyendo inactivos): {changePasswordUI != null}");
        
        if (changePasswordUI != null)
        {
            Debug.Log($"🚀 Llamando ShowPanel con ProfileId: {profileId}");
            changePasswordUI.ShowPanel(profileId);
        }
        else
        {
            Debug.LogError("❌ ChangePasswordUI no encontrado en la escena.");
        }
    }

    public void UpdateProfileInfo()
    {
        var persistence = FindObjectOfType<PersistenceController>();
        bool hasData = false;
        if (persistence != null)
        {
            var data = persistence.GetProfileData(profileId);
            hasData = data != null;
            if (hasData)
            {
                nameText.text = persistence.GetProfileName(profileId);
                healthText.text = "Salud:" + persistence.GetProfileHealth(profileId);
                sceneText.text = persistence.GetProfileScene(profileId);
            }
            else
            {
                nameText.text = $"Perfil {profileId}";
                healthText.text = "-";
                sceneText.text = "-";
            }
        }
        else
        {
            nameText.text = $"Perfil {profileId}";
            healthText.text = "-";
            sceneText.text = "Escena: -";
        }
        if (infoImage != null)
        {
            infoImage.gameObject.SetActive(hasData);
        }
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

    // Agrega este método temporal en ProfileSlotUI para debug:
    [ContextMenu("Find All ChangePasswordUI")]
    public void FindAllChangePasswordUI()
    {
        var allChangePasswordUI = FindObjectsOfType<ChangePasswordUI>();
        Debug.Log($"🔍 Total ChangePasswordUI encontrados: {allChangePasswordUI.Length}");
        
        for (int i = 0; i < allChangePasswordUI.Length; i++)
        {
            Debug.Log($"  [{i}] {allChangePasswordUI[i].name} - Active: {allChangePasswordUI[i].gameObject.activeInHierarchy}");
        }
        
        if (allChangePasswordUI.Length == 0)
        {
            Debug.LogError("❌ NO hay ningún ChangePasswordUI en la escena");
        }
    }
}