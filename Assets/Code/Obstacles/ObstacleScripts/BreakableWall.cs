using TMPro;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public float speedThreshold;

    TextMeshProUGUI speedText;

    void Start()
    {
        speedText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        speedText.text = speedThreshold + "";
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var shoot = collision.collider.GetComponentInParent<PlayerShoot>();
            float playerSpeed = shoot.lastVelocity.magnitude;
            Debug.Log($"player speed={playerSpeed} threshold={speedThreshold} ");
            if(playerSpeed >= speedThreshold) {
                if(GetComponent<ParticleHolder>() != null)
                {
                    GetComponent<ParticleHolder>().useParticle(transform, 0);
                }

                collision.collider.GetComponentInParent<Rigidbody2D>().linearVelocity = shoot.lastVelocity;

                Destroy(gameObject);
                
            }
        }
    }
}
