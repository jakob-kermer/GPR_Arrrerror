using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    public void RestartGame()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Restart");
    }

    public void MainMenu()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Main Menu");
    }

    public void QuitGame()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
        Application.Quit();
        Debug.Log("Quit");
    }
}
