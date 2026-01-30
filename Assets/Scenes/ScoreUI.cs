using UnityEngine;
using TMPro; // если используешь TextMeshPro

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText; // перетащи Text в инспекторе

    private void Update()
    {
        // обновл€ем текст каждый кадр
        scoreText.text = "—чЄт: " + ScoreManager.Instance.score;
    }
}