using UnityEngine;

public class ButtonBullet : MonoBehaviour
{
    public ButtonDoor buttonDoor;
    [Tooltip("Optional channel name shared with a ButtonDoor in a later room.")]
    public string signalId;

    public void SetSignalId(string newSignalId)
    {
        signalId = newSignalId;
    }

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
        RoomSignalBus.Raise(signalId);
    }
}
