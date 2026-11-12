using System.Collections;
using System.IO;
using Assets.Code.Cavas.Shop;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    public Transform Body;
    public Transform OpenPosition;
    public Transform ClosedPosition;

    public float moveSpeed = 2f;
    public float openDuration = 1f;
    [Tooltip("Optional channel name shared with a ButtonBullet in another room.")]
    public string signalId;

    public bool open;

    public void OpenDoor()
    {
        if (OpenPosition == null)
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
        var upgrade = (DoorOpenSpeed)UpgradesManager.Instance.FindRightItem("DoorOpenSpeed");

        Body.position = Vector3.MoveTowards(
                Body.position,
                target,
                Mathf.Max(0f, moveSpeed * upgrade.BaseSpeed) * Time.deltaTime);
    }

}
