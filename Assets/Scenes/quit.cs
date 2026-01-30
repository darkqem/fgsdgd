using UnityEngine;

public class QuitOnEscape : MonoBehaviour
{
    void Update()
    {
        // Если нажата клавиша Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    
    void QuitGame()
    {
        #if UNITY_EDITOR
            // Если в редакторе - останавливаем Play Mode
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Если в финальной сборке - закрываем приложение
            Application.Quit();
        #endif
    }
}