using UnityEngine;

public class ButtonBullet : MonoBehaviour
{
    public ButtonDoor buttonDoor;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (buttonDoor != null)
        {
            buttonDoor.OpenDoor();
        }
    }
}
