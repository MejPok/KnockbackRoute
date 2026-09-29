using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedrunTimer : MonoBehaviour
{
    public static SpeedrunTimer speedrunTimer;
    public TextMeshProUGUI textTimer;
    public float Timer;
    public bool timing;
    void Start()
    {
        speedrunTimer = this;
        timing = true;
    }

    void Update()
    {
        if (timing)
        {
            Timer += Time.deltaTime;
        }
        textTimer.text = "Time: " + Timer;
    }

    public void StopTiming()
    {
        timing = false;
    }



}
