using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BulletInventory))]

public class PlayerShoot : MonoBehaviour
{
    public static PlayerShoot Instance;
    float bigBulletSize = 2f;

    [SerializeField] float bulletSpawnDistance = 0.6f;
    [SerializeField] public float maxVelocity = 4f;

    [SerializeField] bool enableVelocityBoost = true;
    [SerializeField, Range(0f, 1f)] float nearMaxSpeedThreshold = 0.9f;

    public Vector2 lastVelocity;
    BulletInventory bulletInventory;
    Rigidbody2D rb;
    public Vector2 aimDirection;
    [SerializeField] float temporaryVelocityBoost;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        transform.position = new Vector2(0, 1);
        GetComponent<PlayerCorruptionHp>().InsideCorruption = false;
        GetComponent<PlayerCorruptionHp>().Hp = GetComponent<PlayerCorruptionHp>().maxHp;
        bulletInventory = GetComponent<BulletInventory>();
        bulletInventory.NormalBulletsCount = 50;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
    }

    public float bigBuleltHoldTime = 3f;
    float bigBulletHoldTimer;

    public bool ManualMovement;
    public float ManualMovementSpeed;
    
    void Update()
    {
        RotatePlayer();

        SpeedControl();

        if(SceneManager.GetActiveScene().name == "HomeScene")
        {
            ManualMovement = true;
        }
        else
        {
            ManualMovement = false;
        }

        if (ManualMovement)
        {
            ManualMovementInput();
            return;
        }

        foreach (var bulletType in bulletInventory.bulletsEnabled)
        {
        
            if (bulletType.bulletName == "Big Bullet")
            {
                if (Keyboard.current[bulletType.shootKey].isPressed)
                {
                    bigBulletHoldTimer += Time.deltaTime;
                    GetComponentInChildren<ChargeRing>().SetCharge(bigBulletHoldTimer / bigBuleltHoldTime);
                    if (bigBulletHoldTimer >= bigBuleltHoldTime && !bulletInventory.IsBulletOnCooldown(bulletType))
                    {
                        bigBulletHoldTimer = 0;
                        GetComponentInChildren<ChargeRing>().SetCharge(bigBulletHoldTimer / bigBuleltHoldTime);

                        Shoot(bulletType);

                    }

                    break;
                }
                else
                {
                    bigBulletHoldTimer = 0;
                    GetComponentInChildren<ChargeRing>().SetCharge(bigBulletHoldTimer / bigBuleltHoldTime);

                }




                continue;
            }

            if (Keyboard.current != null && Keyboard.current[bulletType.shootKey].wasPressedThisFrame && !bulletInventory.IsBulletOnCooldown(bulletType))
            {
                Shoot(bulletType);
            }
        }


    }

    Vector2 moveVector;
    void ManualMovementInput()
    {
        int up = 0;
        int side = 0;

        if (Keyboard.current.wKey.isPressed)
        {
            up++;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            side++;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            up--;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            side--;
        }

        moveVector = new Vector2(side, up).normalized;

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

    void Shoot(BulletType bulletType)
    {
        if (bulletType.bulletPrefab == null || aimDirection.sqrMagnitude == 0f)
        {
            return;
        }

        if (!bulletInventory.EnoughNormalBullets(bulletType.numberOfBulletsRequested))
        {
            return;
        }

        bulletInventory.NormalBulletsCount -= bulletType.numberOfBulletsRequested;
        bulletInventory.RenewCooldown(bulletType);

        Vector3 spawnPosition = transform.position + (Vector3)(aimDirection * bulletSpawnDistance);
        GameObject bullet = Instantiate(bulletType.bulletPrefab, spawnPosition, transform.rotation);
        bullet.transform.localScale *= bulletType.sizeMultiplier;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb == null)
        {
            Debug.LogError("Bullet prefab must have a Rigidbody2D.");
            Destroy(bullet);
            return;
        }

        bulletRb.linearVelocity = aimDirection * bulletType.bulletSpeed;
        Destroy(bullet, bulletType.lifetime);

        var particle = Instantiate(bulletType.ParticleSystem, bullet.transform.position, bullet.transform.rotation);

        if (bulletType.enableScreenShake)
        {
            CreateScreenShake(bulletType);
        }



        if (enableVelocityBoost && rb.linearVelocity.magnitude >= maxVelocity * nearMaxSpeedThreshold)
        {
            temporaryVelocityBoost += bulletType.velocityBoostPerShot;
        }

        rb.AddForce(-aimDirection * bulletType.RecoilImpulse, ForceMode2D.Impulse);

        SpeedControl();
    }

    void SpeedControl()
    {

        float currentMaxVelocity = maxVelocity + (enableVelocityBoost ? temporaryVelocityBoost : 0f) * (2 - GetComponent<PlayerCorruptionHp>().HpPercentage());
        if (!enableVelocityBoost || rb.linearVelocity.magnitude < currentMaxVelocity * nearMaxSpeedThreshold)
        {
            temporaryVelocityBoost *= 0.99f;
        }

        if (rb.linearVelocity.magnitude > currentMaxVelocity)
        {
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, currentMaxVelocity);
        }

        lastVelocity = rb.linearVelocity;

    }

    void CreateScreenShake(BulletType bulletType)
    {
        if (GetComponent<CinemachineImpulseSource>() != null)
        {
            GetComponent<CinemachineImpulseSource>().GenerateImpulse(bulletType.shakeStrength);
        }
    }

    private void FixedUpdate()
    {
        if (ManualMovement)
        {
            rb.linearVelocity = moveVector * ManualMovementSpeed;
        }
    }



}
