using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] AudioManager audioManager;
    public void PlayGame()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
        SceneManager.LoadSceneAsync(1);
    }

    public void Options()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
    }

    public void QuitGame()
    {
        audioManager.PlaySFX(audioManager.buttonpress);
        Application.Quit();
    }
}