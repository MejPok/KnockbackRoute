using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Door Open Speed")]
public class DoorOpenSpeed : ShopItem
{
    public float BaseSpeed= 1;
    public float IncreasePercentagePerlevel = 0.5f;

    public override void BoughtItem()
    {
        itemLevel++;
        BaseSpeed = 1 +  itemLevel*IncreasePercentagePerlevel;
        base.BoughtItem();
    }
}
