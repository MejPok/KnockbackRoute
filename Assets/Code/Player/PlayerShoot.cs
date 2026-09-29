using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 18f;
    [SerializeField] float recoilImpulse = 12f;
    [SerializeField] float bulletSpawnDistance = 0.6f;
    [SerializeField] float bulletLifetime = 4f;

    Rigidbody2D rb;
    Vector2 aimDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        RotatePlayer();

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void RotatePlayer()
    {
        if (Mouse.current == null || Camera.main == null)
        {
            return;
        }

        var mousePos = Mouse.current.position.ReadValue();
        var worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        aimDirection = (Vector2)worldMousePos - (Vector2)transform.position;
        if (aimDirection.sqrMagnitude > 0f)
        {
            aimDirection.Normalize();
            transform.up = aimDirection;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || aimDirection.sqrMagnitude == 0f)
        {
            return;
        }

        Vector3 spawnPosition = transform.position + (Vector3)(aimDirection * bulletSpawnDistance);

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        
        if (bulletRb == null)
        {
            Debug.LogError("Bullet prefab must have a Rigidbody2D.", bulletPrefab);
            Destroy(bullet);
            return;
        }

        bulletRb.linearVelocity = aimDirection * bulletSpeed;
        Destroy(bullet, bulletLifetime);
        rb.AddForce(-aimDirection * recoilImpulse, ForceMode2D.Impulse);
    }
}
