using System.Collections;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    public Transform Body;
    public Transform OpenPosition;
    public Transform ClosedPosition;

    public float moveSpeed = 2f;
    public float openDuration = 1f;

    public bool open;

    public void OpenDoor()
    {
        if (OpenPosition == null || ClosedPosition == null)
        {
            return;
        }
        open = true;

    }

    public void CloseDoor()
    {
        if (ClosedPosition == null)
        {
            return;
        }
        open = false;
    }

    void Update()
    {
        var target = (open) ? OpenPosition.position : ClosedPosition.position;
        Body.position = Vector3.MoveTowards(
                Body.position,
                target,
                Mathf.Max(0f, moveSpeed) * Time.deltaTime);
    }

}
