using UnityEngine;

public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public string itemDescription;
    public int originalPrice;

    public BulletType BulletType;

    public int itemPrice
    {
        get => originalPrice + (itemLevel * increasePricePerLevel);
    }
    public int itemLevel = 0;
    public int maxItemLevel = 0;


    public int increasePricePerLevel = 10;

    public virtual void BoughtItem()
    {
        Debug.Log($"Used {itemName}");
    }

}
