using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    public void PlayGame()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
        SceneManager.LoadSceneAsync(1);
    }

    public void Options()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
    }

    public void QuitGame()
    {
        audioManager.PlaySFX(audioManager.ButtonPress);
        Application.Quit();
    }
}