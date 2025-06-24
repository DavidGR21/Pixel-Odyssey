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

   // En ProfileButtonUI.cs o ProfileSlotUI.cs
public void OnProfileSelected(int profileId)
{
    ProfileManager.Instance.SetActiveProfile(profileId);
    GameManager.Instance.PlayGame();
}
    public void OnDeleteProfile(int profileId)
    {
        ProfileManager.Instance.DeleteProfile(profileId);

        // Actualiza todos los slots de perfil previene el error de que el slot no se actualice
        ProfileManager.Instance.UpdateAllProfileSlots();
    }
}