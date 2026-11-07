using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Money Gain")]
public class MoneyGain : ShopItem
{
    public float MoneyGainIncreasePercentage = 1;
    public float IncreasePercentagePerlevel = 0.1f;
    public override void BoughtItem()
    {
        itemLevel++;
        MoneyGainIncreasePercentage = 1 + itemLevel * IncreasePercentagePerlevel;
        base.BoughtItem();

    }
}
