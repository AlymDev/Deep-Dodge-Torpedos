using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips")]
    public AudioClip explosionClip;
    public AudioClip coinClip;
    public AudioClip gameOverClip;
    public AudioClip musicClip;

    [Header("Skip the start of each clip (seconds)")]
    public float explosionStartOffset = 1f;
    public float coinStartOffset = 1f;
    public float gameOverStartOffset = 0f;

    [Header("Pitch variation")]
    [Range(0f, 0.5f)] public float pitchVariation = 0.1f;

    [Header("Volumes (0 to 1)")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.4f;

    private AudioSource musicSource;
    private const string SfxKey = "SfxVolume";
    private const string MusicKey = "MusicVolume";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Load saved volumes (use the Inspector values the first time)
        sfxVolume = PlayerPrefs.GetFloat(SfxKey, sfxVolume);
        musicVolume = PlayerPrefs.GetFloat(MusicKey, musicVolume);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;

        if (musicClip != null) musicSource.Play();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        if (musicSource != null) musicSource.volume = value;
        PlayerPrefs.SetFloat(MusicKey, value);
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat(SfxKey, value);
    }

    public void PlayExplosion() { Play(explosionClip, explosionStartOffset); }
    public void PlayCoin()      { Play(coinClip, coinStartOffset); }
    public void PlayGameOver()  { Play(gameOverClip, gameOverStartOffset); }

    void Play(AudioClip clip, float startOffset)
    {
        if (clip == null) return;

        startOffset = Mathf.Clamp(startOffset, 0f, clip.length - 0.05f);
        float pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

        GameObject temp = new GameObject("TempSFX");
        AudioSource src = temp.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = sfxVolume;
        src.pitch = pitch;
        src.spatialBlend = 0f;
        src.time = startOffset;
        src.Play();

        Destroy(temp, (clip.length - startOffset) / pitch + 0.1f);
    }
}