using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Bullet Gain")]
public class BulletGain : ShopItem
{

    public float BulletGainIncreasePercentage = 1;
    public float IncreasePercentagePerlevel = 0.1f;

    public override void BoughtItem()
    {
        itemLevel++;
        BulletGainIncreasePercentage = 1 +  itemLevel*IncreasePercentagePerlevel;
        base.BoughtItem();
    }
}
