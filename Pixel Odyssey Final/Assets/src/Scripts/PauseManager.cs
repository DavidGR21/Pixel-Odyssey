using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public GameObject tittle;
    private bool isGameOver = false;
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

    }
    public void ShowGameOverMenu()
    {
        if (tittle != null)
            tittle.SetActive(true);
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
        if (resumeButton != null)
            resumeButton.SetActive(false);
        Time.timeScale = 0f;
        isGameOver = true;

    }

    private void Update()
    {
        if (isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    public void RestoreLastCheckpoint()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isGameOver = false;
        if (tittle != null)
            tittle.SetActive(false);
        GameManager.Instance.RestoreLevelFromPersistence();

    }
    public void TogglePause()
    {
        if (pauseMenu == null) return;
        bool isPaused = !pauseMenu.activeSelf;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        if (resumeButton != null)
            resumeButton.SetActive(true);
    }
    public void RestartLevel()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitToMenu()
    {
        if (tittle != null)
            tittle.SetActive(false);
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }
}