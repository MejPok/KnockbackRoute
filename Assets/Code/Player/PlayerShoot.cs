using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BulletInventory))]

public class PlayerShoot : MonoBehaviour
{
    public static PlayerShoot Instance;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 18f;
    [SerializeField] float recoilImpulse = 12f;
    [SerializeField] float bulletSpawnDistance = 0.6f;
    [SerializeField] float bulletLifetime = 4f;
    [SerializeField] public float maxVelocity = 4f;
    
    [SerializeField] bool enableVelocityBoost = true;
    [SerializeField, Min(0f)] float velocityBoostPerShot = 1f;
    [SerializeField, Range(0f, 1f)] float nearMaxSpeedThreshold = 0.9f;

    BulletInventory bulletInventory;

    Rigidbody2D rb;
    Vector2 aimDirection;
    [SerializeField] float temporaryVelocityBoost;

    void Start()
    {
        Instance = this;
        bulletInventory = GetComponent<BulletInventory>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        RotatePlayer();

        if (!enableVelocityBoost || rb.linearVelocity.magnitude < maxVelocity * nearMaxSpeedThreshold)
        {
            temporaryVelocityBoost = 0f;
        }

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

        if (!bulletInventory.EnoughNormalBullets())
        {
            return;
        }
        bulletInventory.NormalBulletsCount -= 1;

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

        if (enableVelocityBoost && rb.linearVelocity.magnitude >= maxVelocity * nearMaxSpeedThreshold)
        {
            temporaryVelocityBoost += velocityBoostPerShot;
        }

        rb.AddForce(-aimDirection * recoilImpulse, ForceMode2D.Impulse);

        float currentMaxVelocity = maxVelocity + (enableVelocityBoost ? temporaryVelocityBoost : 0f);
        if (rb.linearVelocity.magnitude > currentMaxVelocity)
        {
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, currentMaxVelocity);
        }


    }
}
