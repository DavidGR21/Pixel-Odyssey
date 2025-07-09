using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
/// <summary>
/// Controlador de la pantalla de carga.
/// Este script se encarga de simular una barra de carga y mostrar el porcentaje de carga.
/// Al finalizar la carga, se cambia a la escena del menú principal.
/// </summary>
public class LoadingController : MonoBehaviour
{
    public Slider barraCarga;
    public TextMeshProUGUI textoPorcentaje;
    public float duracionCarga = 5f;

    void Start()
    {
        StartCoroutine(CargaSimulada());
    }

    IEnumerator CargaSimulada()
    {
        float tiempo = 0f;

        while (tiempo < duracionCarga)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracionCarga);
            barraCarga.value = progreso;

            int porcentajeInt = Mathf.RoundToInt(progreso * 100f);
            textoPorcentaje.text = porcentajeInt + "%";

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }
}
