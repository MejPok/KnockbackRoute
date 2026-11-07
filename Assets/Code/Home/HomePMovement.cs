using UnityEngine;
using UnityEngine.InputSystem;

public class HomePMovement : MonoBehaviour
{
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public float speed;
    Vector2 moveVector;
    void Update()
    {
        int up = 0;
        int side = 0;

        if (Keyboard.current.wKey.isPressed)
        {
            up++;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            up--;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            side++;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            side--;
        }
        moveVector = new Vector2(side, up).normalized;

    }


    private void FixedUpdate()
    {
        
        rb.linearVelocity = moveVector * speed;
    }

}
