using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// Clase encargada de gestionar el botón de perfil en la interfaz de usuario.
/// Permite seleccionar un perfil, eliminarlo y manejar efectos de hover.
/// Se utiliza en conjunto con el ProfileManager para gestionar los perfiles del jugador.
/// </summary>
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
    //Metodo para manejar la selección del perfil
    public void OnProfileSelected(int profileId)
    {
        ProfileManager.Instance.SetActiveProfile(profileId);
        GameManager.Instance.PlayGame();
    }
    // Método para manejar la eliminación del perfil
    public void OnDeleteProfile(int profileId)
    {
        ProfileManager.Instance.DeleteProfile(profileId);

        // Actualiza todos los slots de perfil previene el error de que el slot no se actualice
        ProfileManager.Instance.UpdateAllProfileSlots();
    }
}