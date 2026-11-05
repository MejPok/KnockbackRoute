using UnityEngine;

public class UpgradesPlayerAction : MonoBehaviour
{
    public static UpgradesPlayerAction Instance;
    public PlayerHook playerHook;
    public PlayerShoot playerShoot;
    public BulletInventory bulletInventory;

    private void Start()
    {
        Instance = this;
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
}
