using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Fields
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip buttonPress;
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip death;
    [SerializeField] private AudioClip fireball;
    [SerializeField] private AudioClip shitstorm;
    [SerializeField] private AudioClip block;
    [SerializeField] private AudioClip taunt;
    [SerializeField] private AudioClip heal;
    [SerializeField] private AudioClip cat;
    [SerializeField] private AudioClip potion;

    // Properties
    public AudioClip BackgroundMusic
    {
        get { return backgroundMusic; }
        set { this.backgroundMusic = value; }
    }
    public AudioClip ButtonPress
    {
        get { return buttonPress; }
        set { this.buttonPress = value; }
    }
    public AudioClip Hit
    {
        get { return hit; }
        set { this.hit = value; }
    }
    public AudioClip Death
    {
        get { return death; }
        set { this.death = value; }
    }
    public AudioClip Fireball
    {
        get { return fireball; }
        set { this.fireball = value; }
    }
    public AudioClip Shitstorm
    {
        get { return shitstorm; }
        set { this.shitstorm = value; }
    }
    public AudioClip Block
    {
        get { return block; }
        set { this.block = value; }
    }
    public AudioClip Taunt
    {
        get { return taunt; }
        set { this.taunt = value; }
    }
    public AudioClip Heal
    {
        get { return heal; }
        set { this.heal = value; }
    }
    public AudioClip Cat
    {
        get { return cat; }
        set { this.cat = value; }
    }
    public AudioClip Potion
    {
        get { return potion; }
        set { this.potion = value; }
    }

    private void Start()
    {
        musicSource.clip = BackgroundMusic;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
