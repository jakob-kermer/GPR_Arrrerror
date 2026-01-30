using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Audio Sources
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    //Audio Clips
    public AudioClip background;
    public AudioClip death;
    public AudioClip hit;
    public AudioClip buttonpress;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
