using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Enable Big Bullet")]
public class EnableBigBullet  : ShopItem
{
    public override void BoughtItem()
    {
        itemLevel++;
        PlayerShoot.Instance.GetComponent<UpgradesPlayerAction>().EnableBigBullet();

        base.BoughtItem();
        
    }
}
