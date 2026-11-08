using UnityEngine;

public class UpgradesPlayerAction : MonoBehaviour
{
    public PlayerHook playerHook;
    public PlayerShoot playerShoot;
    public BulletInventory bulletInventory;

    private void Start()
    {
        playerHook = GetComponent<PlayerHook>();
        playerShoot = GetComponent<PlayerShoot>();
        bulletInventory = GetComponent<BulletInventory>();
    }

    public void SmallerBigBulletCooldown(SmallerBigBulletCooldown smallerBigBulletCooldown)
    {
        foreach(BulletType bulletType in bulletInventory.allBulletsType)
        {
            if(smallerBigBulletCooldown.BulletType.name == bulletType.name)
            {
                bulletType.CD -= smallerBigBulletCooldown.cooldownReductionPerLevel;
                if(bulletType.CD < 0f)
                {
                    bulletType.CD = 0f;
                }
                break;
            }
        }
        
    }

    public void EnableBigBullet()
    {
        foreach (BulletType bulletType in bulletInventory.allBulletsType)
        {
            if (bulletType.bulletName == "Big Bullet")
            {
                if (!bulletInventory.bulletsEnabled.Contains(bulletType))
                {
                    bulletInventory.bulletsEnabled.Add(bulletType);
                    Debug.Log("Added big bullet to inventory");
                    return;
                }
            }
        }
        Debug.Log("Couldnt find big bullet item");

    }
}
