using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedShow : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public Slider speedSlider;
    PlayerShoot player;

    void Start()
    {
        player = PlayerShoot.Instance;
    }

    float timeSinceLastJump;
    public float jumpCooldown;
    bool jumpCooldownOn;

    void Update()
    {
        if (jumpCooldownOn)
        {
            timeSinceLastJump += Time.deltaTime;

            if(timeSinceLastJump >= jumpCooldown)
            {
                jumpCooldownOn = false;
            }
        }

        speedSlider.maxValue = player.maxVelocity;
        speedSlider.value = player.GetComponent<Rigidbody2D>().linearVelocity.magnitude;
        speedSlider.minValue = 0;

        speedText.text = "" + (int)player.GetComponent<Rigidbody2D>().linearVelocity.magnitude;
    }
}
