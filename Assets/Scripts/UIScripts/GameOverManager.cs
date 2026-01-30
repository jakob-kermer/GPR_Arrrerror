using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] AudioManager audioManager;
    public void RestartGame()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Restart");
    }

    public void MainMenu()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Main Menu");
    }

    public void QuitGame()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
        Application.Quit();
        Debug.Log("Quit");
    }
}
