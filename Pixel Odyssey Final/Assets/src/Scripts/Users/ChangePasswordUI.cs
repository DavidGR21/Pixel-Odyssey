using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Interfaz de usuario para el cambio de contraseñas.
/// Maneja la interacción del usuario y muestra mensajes de resultado.
/// </summary>
public class ChangePasswordUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject changePasswordPanel;
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public Button changePasswordButton;
    public Button cancelButton;
    public TMP_Text messageText;
    public TMP_Text titleText;

    [Header("Configuration")]
    public int currentProfileId = 1;

    private PersistenceController persistenceController;
    private bool isSettingInitialPassword = false;

    private void Start()
    {
        Debug.Log("🔧 ChangePasswordUI: Start() iniciado");
        
        persistenceController = FindObjectOfType<PersistenceController>();
        Debug.Log($"🔧 PersistenceController encontrado: {persistenceController != null}");
        
        SetupUI();
        CheckPasswordStatus();
        
        Debug.Log("🔧 ChangePasswordUI: Start() completado");
    }

    private void SetupUI()
    {
        Debug.Log("🔧 SetupUI iniciado");
        
        if (changePasswordButton != null)
        {
            changePasswordButton.onClick.AddListener(OnChangePasswordClicked);
            Debug.Log("✅ ChangePasswordButton configurado");
        }
        else
        {
            Debug.LogError("❌ ChangePasswordButton es NULL");
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelClicked);
            Debug.Log("✅ CancelButton configurado");
        }
        else
        {
            Debug.LogWarning("⚠️ CancelButton es NULL");
        }

        // Configurar inputs como contraseña
        SetupPasswordInput(currentPasswordInput);
        SetupPasswordInput(newPasswordInput);
        SetupPasswordInput(confirmPasswordInput);

        HidePanel();
        Debug.Log("🔧 SetupUI completado");
    }

    private void SetupPasswordInput(TMP_InputField input)
    {
        if (input != null)
        {
            input.contentType = TMP_InputField.ContentType.Password;
            Debug.Log($"✅ Input configurado: {input.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ Input es NULL");
        }
    }

    private void CheckPasswordStatus()
    {
        Debug.Log($"🔧 CheckPasswordStatus: ProfileId={currentProfileId}");
        
        if (persistenceController != null)
        {
            isSettingInitialPassword = !persistenceController.HasPassword(currentProfileId);
            Debug.Log($"🔧 IsSettingInitialPassword: {isSettingInitialPassword}");
            
            if (isSettingInitialPassword)
            {
                if (currentPasswordInput != null)
                {
                    currentPasswordInput.gameObject.SetActive(false);
                    Debug.Log("🔧 CurrentPasswordInput ocultado (primera vez)");
                }
                    
                if (titleText != null)
                {
                    titleText.text = "Establecer Contraseña";
                    Debug.Log("🔧 Título cambiado a 'Establecer Contraseña'");
                }
            }
            else
            {
                if (currentPasswordInput != null)
                {
                    currentPasswordInput.gameObject.SetActive(true);
                    Debug.Log("🔧 CurrentPasswordInput mostrado");
                }
                    
                if (titleText != null)
                {
                    titleText.text = "Cambiar Contraseña";
                    Debug.Log("🔧 Título cambiado a 'Cambiar Contraseña'");
                }
            }
        }
        else
        {
            Debug.LogError("❌ PersistenceController es NULL en CheckPasswordStatus");
        }
    }

    public void ShowPanel(int profileId)
    {
        Debug.Log($"🚀 ShowPanel llamado con ProfileId: {profileId}");
        
        currentProfileId = profileId;
        CheckPasswordStatus();
        
        if (changePasswordPanel != null)
        {
            Debug.Log($"🔧 Panel state antes: {changePasswordPanel.activeInHierarchy}");
            changePasswordPanel.SetActive(true);
            Debug.Log($"🔧 Panel state después: {changePasswordPanel.activeInHierarchy}");
            Debug.Log("✅ Panel mostrado correctamente");
        }
        else
        {
            Debug.LogError("❌ ChangePasswordPanel es NULL - No se puede mostrar el panel");
        }
        
        ClearInputs();
        ClearMessage();
        
        Debug.Log("🚀 ShowPanel completado");
    }

    public void HidePanel()
    {
        Debug.Log("🔧 HidePanel llamado");
        
        if (changePasswordPanel != null)
        {
            changePasswordPanel.SetActive(false);
            Debug.Log("✅ Panel ocultado");
        }
        else
        {
            Debug.LogError("❌ ChangePasswordPanel es NULL en HidePanel");
        }
        
        ClearInputs();
        ClearMessage();
    }

    private void OnChangePasswordClicked()
    {
        Debug.Log("🔘 OnChangePasswordClicked ejecutado");
        
        if (!ValidateInputs())
        {
            Debug.Log("❌ Validación de inputs falló");
            return;
        }

        string currentPassword = currentPasswordInput != null ? currentPasswordInput.text : "";
        string newPassword = newPasswordInput.text;
        string errorMessage;

        Debug.Log($"🔧 Intentando cambiar contraseña para perfil {currentProfileId}");
        Debug.Log($"🔧 Es primera vez: {isSettingInitialPassword}");

        bool success;

        if (isSettingInitialPassword)
        {
            success = persistenceController.SetInitialPassword(currentProfileId, newPassword, out errorMessage);
        }
        else
        {
            success = persistenceController.ChangePassword(currentProfileId, currentPassword, newPassword, out errorMessage);
        }

        if (success)
        {
            ShowSuccessMessage(isSettingInitialPassword ? "Contraseña establecida exitosamente." : "Contraseña cambiada exitosamente.");
            Invoke(nameof(HidePanel), 2f);
        }
        else
        {
            ShowErrorMessage(errorMessage);
        }
    }

    private void OnCancelClicked()
    {
        Debug.Log("🔘 OnCancelClicked ejecutado");
        HidePanel();
    }

    private bool ValidateInputs()
    {
        Debug.Log("🔧 ValidateInputs iniciado");
        
        // Validar contraseña actual solo si no es la primera vez
        if (!isSettingInitialPassword && (currentPasswordInput == null || string.IsNullOrEmpty(currentPasswordInput.text)))
        {
            ShowErrorMessage("Debes ingresar tu contraseña actual.");
            return false;
        }

        if (newPasswordInput == null || string.IsNullOrEmpty(newPasswordInput.text))
        {
            ShowErrorMessage("Debes ingresar una nueva contraseña.");
            return false;
        }

        if (confirmPasswordInput == null || string.IsNullOrEmpty(confirmPasswordInput.text))
        {
            ShowErrorMessage("Debes confirmar tu nueva contraseña.");
            return false;
        }

        if (newPasswordInput.text != confirmPasswordInput.text)
        {
            ShowErrorMessage("Las contraseñas no coinciden.");
            return false;
        }

        if (newPasswordInput.text.Length < 6)
        {
            ShowErrorMessage("La nueva contraseña debe tener al menos 6 caracteres.");
            return false;
        }

        Debug.Log("✅ Validación de inputs exitosa");
        return true;
    }

    private void ShowSuccessMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = Color.green;
        }
        Debug.Log($"✅ {message}");
    }

    private void ShowErrorMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = Color.red;
        }
        Debug.Log($"❌ {message}");
    }

    private void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    private void ClearInputs()
    {
        if (currentPasswordInput != null) currentPasswordInput.text = "";
        if (newPasswordInput != null) newPasswordInput.text = "";
        if (confirmPasswordInput != null) confirmPasswordInput.text = "";
    }

    // Método de debug manual para probar
    [ContextMenu("Test Show Panel")]
    public void TestShowPanel()
    {
        Debug.Log("🧪 Test manual de ShowPanel");
        ShowPanel(1);
    }

    [ContextMenu("Check Components")]
    public void CheckComponents()
    {
        Debug.Log("🔍 === VERIFICACIÓN DE COMPONENTES ===");
        Debug.Log($"ChangePasswordPanel: {(changePasswordPanel != null ? "✅ OK" : "❌ NULL")}");
        Debug.Log($"CurrentPasswordInput: {(currentPasswordInput != null ? "✅ OK" : "❌ NULL")}");
        Debug.Log($"NewPasswordInput: {(newPasswordInput != null ? "✅ OK" : "❌ NULL")}");
        Debug.Log($"ConfirmPasswordInput: {(confirmPasswordInput != null ? "✅ OK" : "❌ NULL")}");
        Debug.Log($"ChangePasswordButton: {(changePasswordButton != null ? "✅ OK" : "❌ NULL")}");
        Debug.Log($"CancelButton: {(cancelButton != null ? "✅ OK" : "❌ NULL")}");
        Debug.Log($"MessageText: {(messageText != null ? "✅ OK" : "❌ NULL")}");
        Debug.Log($"TitleText: {(titleText != null ? "✅ OK" : "❌ NULL")}");
        Debug.Log($"PersistenceController: {(persistenceController != null ? "✅ OK" : "❌ NULL")}");
        
        if (changePasswordPanel != null)
        {
            Debug.Log($"Panel Active: {changePasswordPanel.activeInHierarchy}");
            Debug.Log($"Panel GameObject: {changePasswordPanel.name}");
        }
    }
}