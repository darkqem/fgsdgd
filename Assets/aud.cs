using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance;
    
    [Header("Музыкальные треки")]
    public AudioClip[] musicTracks; // Массив треков для плейлиста
    public bool shuffle = true; // Перемешивать треки
    public bool loopPlaylist = true; // Зациклить плейлист
    
    [Header("Настройки громкости")]
    public float musicVolume = 0.5f;
    public float fadeDuration = 2f; // Длительность fade эффекта
    
    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private bool isFading = false;
    
    void Awake()
    {
        // Singleton pattern для доступа из других скриптов
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Музыка продолжается между сценами
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        audioSource = GetComponent<AudioSource>();
        SetupAudioSource();
    }
    
    void SetupAudioSource()
    {
        audioSource.playOnAwake = true;
        audioSource.loop = false; // Отключаем loop, т.к. будем управлять плейлистом
        audioSource.volume = musicVolume;
        audioSource.spatialBlend = 0f; // 2D звук, не зависит от позиции в пространстве
        audioSource.priority = 0; // Высший приоритет
    }
    
    void Start()
    {
        if (musicTracks.Length > 0)
        {
            PlayNextTrack();
        }
    }
    
    void Update()
    {
        // Автопереключение треков
        if (!audioSource.isPlaying && !isFading && musicTracks.Length > 0)
        {
            PlayNextTrack();
        }
    }
    
    public void PlayNextTrack()
    {
        if (musicTracks.Length == 0) return;
        
        if (shuffle)
        {
            currentTrackIndex = Random.Range(0, musicTracks.Length);
        }
        else
        {
            currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        }
        
        audioSource.clip = musicTracks[currentTrackIndex];
        StartCoroutine(FadeInAndPlay(fadeDuration));
    }
    
    private System.Collections.IEnumerator FadeInAndPlay(float duration)
    {
        isFading = true;
        
        // Fade in
        float timer = 0f;
        audioSource.volume = 0f;
        audioSource.Play();
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, musicVolume, timer / duration);
            yield return null;
        }
        
        audioSource.volume = musicVolume;
        isFading = false;
    }
    
    public void PauseMusic()
    {
        StartCoroutine(FadeOut(fadeDuration, true));
    }
    
    public void ResumeMusic()
    {
        if (!audioSource.isPlaying)
        {
            StartCoroutine(FadeInAndPlay(fadeDuration));
        }
    }
    
    public void StopMusic()
    {
        StartCoroutine(FadeOut(fadeDuration, false));
    }
    
    private System.Collections.IEnumerator FadeOut(float duration, bool pauseAfterFade)
    {
        isFading = true;
        
        float startVolume = audioSource.volume;
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }
        
        audioSource.volume = 0f;
        
        if (pauseAfterFade)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Stop();
        }
        
        isFading = false;
    }
    
    // Для изменения громкости из UI или настроек
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (!isFading)
        {
            audioSource.volume = musicVolume;
        }
    }
}