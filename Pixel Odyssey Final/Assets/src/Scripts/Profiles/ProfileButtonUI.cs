using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProfileButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int profileId;
    public Button deleteButton;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Efecto hover (por ejemplo, cambiar color o escalar)
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Quitar efecto hover
    }

    public void OnSelectProfile()
    {
        ProfileManager.Instance.SetActiveProfile(profileId);
        // Lógica para cargar el perfil seleccionado
    }

    public void OnDeleteProfile()
    {
        ProfileManager.Instance.DeleteProfile(profileId);
        GetComponent<ProfileSlotUI>().UpdateProfileInfo();
    }
}