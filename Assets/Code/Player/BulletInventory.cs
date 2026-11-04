using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletInventory : MonoBehaviour
{
    public BulletType[] allBulletsType;
    public List<BulletType> bulletsEnabled = new List<BulletType>();
    public List<float> bulletCooldowns = new List<float>();


    public int NormalBulletsCount;
    public bool limitedBulletsEnabled;

    void Update()
    {
        if(bulletsEnabled.Count > bulletCooldowns.Count)
        {
            bulletCooldowns.Add(0f);
        }

        for(int i = 0; i < bulletCooldowns.Count; i++)
        {
            if (bulletCooldowns[i] > 0f)
            {
                bulletCooldowns[i] -= Time.deltaTime;
            }
        }
    }

    public void RenewCooldown(BulletType bulletType)
    {
        int index = bulletsEnabled.IndexOf(bulletType);
        if (index >= 0 && index < bulletCooldowns.Count)
        {
            bulletCooldowns[index] = bulletType.CD;
        }
    }

    public bool IsBulletOnCooldown(BulletType bulletType)
    {
        int index = bulletsEnabled.IndexOf(bulletType);
        if (index >= 0 && index < bulletCooldowns.Count)
        {
            return bulletCooldowns[index] > 0f;
        }
        return false;
    }

    public bool EnoughNormalBullets(int requiredBullets)
    {
        if (!limitedBulletsEnabled || NormalBulletsCount >= requiredBullets)
        {
            return true;
        }

        return false;
    }

    public bool EnableNewBulletType(string bulletType)
    {
        foreach(BulletType bullet in allBulletsType)
        {
            if (bullet.bulletName == bulletType)
            {
                foreach (BulletType enabledBullet in bulletsEnabled)
                {
                    if (enabledBullet.bulletName == bulletType)
                    {
                        return false;
                    }
                }

                bool bulletFound = false;

                for (int i = 0; i < allBulletsType.Length; i++)
                {
                    if (allBulletsType[i].name == bulletType)
                    {
                        bulletsEnabled.Add(allBulletsType[i]);
                        bulletFound = true;
                    }
                    
                }
                
                return bulletFound;
            }
        }
        return false;
    }
}
