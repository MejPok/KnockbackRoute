using UnityEngine;

public class ShowSwingDirection : MonoBehaviour
{
    public Rigidbody2D playerRb;
    LineRenderer line;

    public float lookAhead;
    public float arrowHeadSize;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 4;
        line.useWorldSpace = true;
        playerRb = transform.parent.GetComponent<Rigidbody2D>();
    }
    private void LateUpdate() {

        if (!PlayerShoot.Instance.gameObject.GetComponent<PlayerHook>().isHooked)
        {
            return;
        }
        
        Vector3 start = playerRb.position;
        Vector3 velocity = playerRb.linearVelocity;

        Vector3 end = start + velocity * lookAhead;

        if(velocity.sqrMagnitude < 0.01f)
        {
            line.enabled = false;
            return;
        }

        line.enabled = true;

        Vector3 direction = velocity.normalized;
        Vector3 side = Vector3.Cross(direction, Vector3.up).normalized;
        if (side.sqrMagnitude < 0.01f)
            side = Vector3.Cross(direction, Vector3.right).normalized;

        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.SetPosition(2, end - direction * arrowHeadSize + side * arrowHeadSize * 0.5f);
        line.SetPosition(3, end - direction * arrowHeadSize - side * arrowHeadSize * 0.5f);
    }
}
