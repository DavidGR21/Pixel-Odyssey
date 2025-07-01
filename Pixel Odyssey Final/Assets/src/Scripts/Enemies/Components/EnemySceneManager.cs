using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
/// <summary>
/// Clase encargada de manejar la escena de los enemigos.
/// Esta clase permite cargar la escena de créditos después de un tiempo determinado
/// y destruir el objeto enemigo después de una animación de muerte.
/// Debe ser utilizada por cualquier clase que implemente un enemigo en el juego.
/// </summary>
public class EnemySceneManager : MonoBehaviour
{
    public void LoadCreditsScene()
    {
        StartCoroutine(LoadCreditsSceneCoroutine());
    }

    public void DestroyAfterDeathAnimation()
    {
        StartCoroutine(DestroyAfterDeathAnimationCoroutine());
    }

    private IEnumerator LoadCreditsSceneCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Credits");
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator DestroyAfterDeathAnimationCoroutine()
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }
}