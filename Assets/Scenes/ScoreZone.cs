using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            ScoreManager.Instance.AddPoint();
        }
    }
}