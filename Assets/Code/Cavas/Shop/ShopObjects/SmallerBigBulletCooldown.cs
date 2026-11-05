using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Smaller Big Bullet Cooldown")]
public class SmallerBigBulletCooldown  : ShopItem
{
    public float cooldownReductionPerLevel;
    public override void BoughtItem()
    {
        itemLevel++;
        UpgradesPlayerAction.Instance.SmallerBigBulletCooldown(this);
        base.BoughtItem();
        
    }
}
