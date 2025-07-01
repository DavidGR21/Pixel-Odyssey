using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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