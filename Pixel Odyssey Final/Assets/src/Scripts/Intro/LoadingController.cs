using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    public Slider barraCarga;
    public TextMeshProUGUI textoPorcentaje;
    public float duracionCarga = 3f;

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

            // Mostrar el porcentaje
            int porcentajeInt = Mathf.RoundToInt(progreso * 100f);
            textoPorcentaje.text = porcentajeInt + "%";

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }
}
