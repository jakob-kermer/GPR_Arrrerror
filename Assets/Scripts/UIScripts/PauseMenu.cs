using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] AudioManager audioManager;

    public void Pause()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void Quit()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Main Menu");
        Time.timeScale = 1;
    }
}
