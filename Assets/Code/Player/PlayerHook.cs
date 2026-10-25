using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHook : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer = 1 << 7;
    [SerializeField] private float hookRange = 12f;

    public float hookStamina;
    public float maxHookStamina;

    private Rigidbody2D rb;
    private PlayerShoot playerShoot;
    private LineRenderer ropeLine;
    private Vector2 hookPoint;
    private float ropeLength;
    public bool isHooked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerShoot = GetComponent<PlayerShoot>();
        ropeLine = GetComponent<LineRenderer>();

        if (ropeLine == null)
            ropeLine = gameObject.AddComponent<LineRenderer>();

        ropeLine.positionCount = 2;
        ropeLine.useWorldSpace = true;
        ropeLine.enabled = false;
    }

    private void Update()
    {
        HookStamina();

        if (Keyboard.current == null || !Keyboard.current.eKey.isPressed)
        {
            isHooked = false;
            ropeLine.enabled = false;
            return;
        }

        if (!isHooked && playerShoot != null && hookStamina >= 0.1f)
            TryHook();
    }

    void HookStamina()
    {
        if (isHooked)
        {
            hookStamina -= Time.deltaTime;
        } else
        {
            hookStamina += Time.deltaTime;
            if(hookStamina >= maxHookStamina)
            {
                hookStamina = maxHookStamina;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isHooked)
            return;

        Vector2 radius = rb.position - hookPoint;
        Vector2 outward = radius.normalized;
        Vector2 tangent = rb.linearVelocity - Vector2.Dot(rb.linearVelocity, outward) * outward;

        if (tangent.sqrMagnitude > 0f)
            rb.linearVelocity = tangent.normalized * rb.linearVelocity.magnitude;

        Vector2 nextPosition = rb.position + rb.linearVelocity * Time.fixedDeltaTime;
        Vector2 nextRadius = nextPosition - hookPoint;
        rb.MovePosition(hookPoint + nextRadius.normalized * ropeLength);
    }

    private void LateUpdate()
    {
        if (!isHooked)
            return;

        ropeLine.SetPosition(0, hookPoint);
        ropeLine.SetPosition(1, rb.position);
    }

    private void TryHook()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, playerShoot.aimDirection, hookRange, wallLayer);
        if (!hit)
            return;

        hookPoint = hit.point;
        ropeLength = Vector2.Distance(rb.position, hookPoint);
        if (ropeLength == 0f)
            return;

        isHooked = true;
        ropeLine.enabled = true;
    }
}
