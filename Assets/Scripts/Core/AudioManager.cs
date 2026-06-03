using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Combat SFX")]
    public AudioClip sfxPunch;
    public AudioClip sfxKick;
    public AudioClip sfxSlam;
    public AudioClip sfxSpecial;

    [Header("Enemy SFX")]
    public AudioClip sfxEnemyHurt;
    public AudioClip sfxEnemyDeath;
    public AudioClip sfxProjectile;

    [Header("Player SFX")]
    public AudioClip sfxPlayerHurt;

    [Header("Music")]
    public AudioClip backgroundMusic;
    private AudioSource musicSource;

    private AudioSource sfxSource;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create two audio sources
        // One for SFX, one for music
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.volume = 0.4f;
    }

    void Start()
    {
        PlayMusic();
    }

    // ─── PLAY METHODS ─────────────────────────────
    public void PlayPunch()
    {
        PlaySFX(sfxPunch, 1f);
    }

    public void PlayKick()
    {
        PlaySFX(sfxKick, 1f);
    }

    public void PlaySlam()
    {
        PlaySFX(sfxSlam, 1.2f);
    }

    public void PlaySpecial()
    {
        PlaySFX(sfxSpecial, 1f);
    }

    public void PlayEnemyHurt()
    {
        PlaySFX(sfxEnemyHurt, 1f);
    }

    public void PlayEnemyDeath()
    {
        PlaySFX(sfxEnemyDeath, 1f);
    }

    public void PlayProjectile()
    {
        PlaySFX(sfxProjectile, 0.8f);
    }

    public void PlayPlayerHurt()
    {
        PlaySFX(sfxPlayerHurt, 1f);
    }

    // ─── CORE ─────────────────────────────────────
    void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    void PlayMusic()
    {
        if (backgroundMusic == null) return;
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
}