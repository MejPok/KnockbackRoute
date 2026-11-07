using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject shopImageShow;
    public Transform[] ShopPositions;
    public ShopItem[] ShopItems;
    void Start() {
        List<ShopItem> shopItems = new List<ShopItem>(ShopItems);

        foreach (Transform pos in ShopPositions)
        {
            int randomIndex = Random.Range(0, shopItems.Count);

            var bulletInventory = PlayerShoot.Instance.gameObject.GetComponent<BulletInventory>();

            if (ShopItems[randomIndex].BulletType != null) //jestli je na to potreba bullettype tak musi byt enabled jinak se to neukaze
            {
                if(!bulletInventory.bulletsEnabled.Contains(ShopItems[randomIndex].BulletType))
                {
                    shopItems.RemoveAt(randomIndex);
                    continue;
                }
            }

            if(randomIndex >= shopItems.Count)
            {
                break;
            }

            if(ShopItems[randomIndex].DontShowThisProductAnymore == true)
            {
                continue;
            }

            GameObject shopItem = Instantiate(shopImageShow, pos.position, Quaternion.identity);
            shopItem.GetComponent<ShopWorldItem>().representedItem = (ShopItem)shopItems[randomIndex];
            shopItem.GetComponent<ShopWorldItem>().shopManager = this;

            shopItems.RemoveAt(randomIndex);
            Debug.Log($"Added {shopItem.GetComponent<ShopWorldItem>().representedItem.itemName} to shop");
        }
    }


}

