using Assets.Code.Round;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            SpeedrunTimer.speedrunTimer.StopTiming();
            RunManager.Instance.RunCompleted(true);
            SpeedrunTimer.speedrunTimer.GetComponent<WinScreenControl>().PlayerWin();
            Debug.Log(SpeedrunTimer.speedrunTimer.Timer + "s the run is completed, Congrats");
        }
    }
}
