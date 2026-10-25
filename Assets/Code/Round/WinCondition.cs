using UnityEngine;

public class WinCondition : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            SpeedrunTimer.speedrunTimer.StopTiming();
            SpeedrunTimer.speedrunTimer.GetComponent<WinScreenControl>().PlayerWin();
            Debug.Log(SpeedrunTimer.speedrunTimer.Timer + "s the run is completed, Congrats");
        }
    }
}
