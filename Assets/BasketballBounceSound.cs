using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
public class BasketballBounceSound : MonoBehaviour
{
    private AudioSource audioSource;
    private Rigidbody rb;
    
    [Header("Sound Settings")]
    public AudioClip[] bounceSounds; // Массив звуков для вариативности
    public float minVelocity = 0.5f; // Минимальная скорость для воспроизведения звука
    public float maxVelocity = 5f; // Максимальная скорость для reference громкости
    
    [Header("Pitch Variation")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        
        // Настройки AudioSource
        audioSource.spatialBlend = 1f; // Полностью 3D звук
        audioSource.playOnAwake = false;
        audioSource.maxDistance = 20f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Получаем скорость столкновения
        float collisionVelocity = collision.relativeVelocity.magnitude;
        
        // Проверяем, достаточно ли сильное столкновение
        if (collisionVelocity > minVelocity && bounceSounds.Length > 0)
        {
            // Выбираем случайный звук
            AudioClip clip = bounceSounds[Random.Range(0, bounceSounds.Length)];
            
            // Настраиваем громкость в зависимости от скорости
            float volume = Mathf.Clamp01(collisionVelocity / maxVelocity);
            
            // Настраиваем случайный pitch
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            
            // Воспроизводим звук
            audioSource.PlayOneShot(clip, volume);
            
            // Дополнительно: можно добавить эффекты в зависимости от поверхности
            CheckSurfaceType(collision.gameObject, volume);
        }
    }
    
    void CheckSurfaceType(GameObject surface, float baseVolume)
    {
        // Можно добавить разные звуки для разных поверхностей
        if (surface.CompareTag("Wood"))
        {
            // Дополнительная обработка для деревянных поверхностей
            audioSource.pitch *= 1.1f;
        }
        else if (surface.CompareTag("Metal"))
        {
            // Для металлических поверхностей
            audioSource.pitch *= 1.2f;
        }
    }
}