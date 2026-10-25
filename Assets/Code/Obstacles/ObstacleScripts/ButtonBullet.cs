using UnityEngine;

public class ButtonBullet : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        GetComponent<SignalPairing>().InvokeSignal();
        
    }
}
