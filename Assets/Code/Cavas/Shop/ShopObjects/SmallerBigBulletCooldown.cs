using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Smaller Big Bullet Cooldown")]
public class SmallerBigBulletCooldown  : ShopItem
{
    public float cooldownReductionPerLevel;
    public override void BoughtItem()
    {
        itemLevel++;
        PlayerShoot.Instance.GetComponent<UpgradesPlayerAction>().SmallerBigBulletCooldown(this);
        base.BoughtItem();
        
    }
}
