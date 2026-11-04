using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletInventory : MonoBehaviour
{
    public BulletType[] allBulletsType;
    public List<BulletType> bulletsEnabled = new List<BulletType>();

    public int NormalBulletsCount;
    public bool limitedBulletsEnabled;

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
