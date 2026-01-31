using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    public void Pause()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void Quit()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Main Menu");
        Time.timeScale = 1;
    }
}
