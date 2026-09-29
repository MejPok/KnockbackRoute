using UnityEngine;

public class BulletInventory : MonoBehaviour
{
    public int NormalBulletsCount;
    public bool limitedBulletsEnabled;

    public bool EnoughNormalBullets()
    {
        if (!limitedBulletsEnabled || NormalBulletsCount > 0)
        {
            return true;
        }

        return false;
    }
}
