using UnityEngine;

public class WinCondition : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            SpeedrunTimer.speedrunTimer.StopTiming();
        }
    }
}
