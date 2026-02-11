using UnityEngine;

public class BallRespawn : MonoBehaviour
{
    [Header("Настройки респавна")]
    [Tooltip("Координата Y, ниже которой мяч телепортируется обратно")]
    public float respawnHeightY = 2f;
    
    [Tooltip("Стартовая позиция мяча (если не задана, используется позиция при старте)")]
    public Vector3 startPosition;
    
    [Tooltip("Задержка перед телепортацией в секундах (0 = мгновенно)")]
    public float respawnDelay = 0f;
    
    [Header("Опционально")]
    [Tooltip("Сбрасывать скорость мяча при телепортации")]
    public bool resetVelocity = true;
    
    [Tooltip("Включить/выключить логирование в консоль")]
    public bool debugMode = false;
    
    // Приватные переменные
    private Rigidbody rb;
    private bool isRespawning = false;
    private float respawnTimer = 0f;
    
    void Start()
    {
        // Получаем компонент Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // Если стартовая позиция не задана в инспекторе, используем текущую
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.position;
            if (debugMode) Debug.Log($"Стартовая позиция установлена: {startPosition}");
        }
    }
    
    void Update()
    {
        // Проверяем, ниже ли мяч заданной высоты
        if (transform.position.y < respawnHeightY && !isRespawning)
        {
            if (respawnDelay <= 0)
            {
                TeleportBall();
            }
            else
            {
                // Запускаем таймер задержки
                isRespawning = true;
                respawnTimer = respawnDelay;
                
                if (debugMode) Debug.Log($"Мяч ниже {respawnHeightY}. Телепортация через {respawnDelay} секунд...");
            }
        }
        
        // Обработка таймера задержки
        if (isRespawning)
        {
            respawnTimer -= Time.deltaTime;
            
            if (respawnTimer <= 0)
            {
                TeleportBall();
                isRespawning = false;
            }
        }
        
        // Сброс флага, если мяч вернулся выше порога самостоятельно
        if (transform.position.y >= respawnHeightY && isRespawning)
        {
            isRespawning = false;
            if (debugMode) Debug.Log("Мяч вернулся выше порога, телепортация отменена");
        }
    }
    
    // Метод для мгновенной телепортации (можно вызвать из другого скрипта)
    public void TeleportBall()
    {
        // Телепортируем мяч на стартовую позицию
        transform.position = startPosition;
        
        // Сбрасываем скорость, если нужно
        if (resetVelocity && rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        if (debugMode) Debug.Log($"Мяч телепортирован на {startPosition}");
        
        isRespawning = false;
    }
    
    // Метод для изменения стартовой позиции (например, если игрок забил с новой позиции)
    public void SetStartPosition(Vector3 newPosition)
    {
        startPosition = newPosition;
        if (debugMode) Debug.Log($"Новая стартовая позиция: {startPosition}");
    }
    
    // Визуализация в редакторе Unity
    void OnDrawGizmosSelected()
    {
        // Рисуем красную линию на уровне респавна
        Gizmos.color = Color.red;
        Vector3 lineStart = new Vector3(transform.position.x - 5f, respawnHeightY, transform.position.z);
        Vector3 lineEnd = new Vector3(transform.position.x + 5f, respawnHeightY, transform.position.z);
        Gizmos.DrawLine(lineStart, lineEnd);
        
        // Рисуем зеленую сферу на стартовой позиции
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPosition, 0.5f);
    }
}