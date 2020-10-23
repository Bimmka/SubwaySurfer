using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;

    private bool dead = false;

    public static PauseMenu instance;
    private void Awake()
    {
        if (PauseMenu.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        PauseMenu.instance = this;

    }
    private void OnDestroy()
    {
        PauseMenu.instance = null;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !dead)
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
        if (dead)
        {
            deathMenu.SetActive(true);
        }
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

    }
    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Subway");
    }
    public void SetDead()
    {
        dead = true;
    }
}
