using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Smaller Big Bullet Cooldown")]
public class BulletGain : ShopItem
{
    public float BulletGainIncrease = 1;
    public override void BoughtItem()
    {
        base.BoughtItem();
        itemLevel++;
    }
}
