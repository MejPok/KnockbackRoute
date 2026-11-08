using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ShopManager : MonoBehaviour
{
    public GameObject shopImageShow;
    public Transform[] ShopPositions;
    public ShopItem[] ShopItems;
    void Start() {
        List<ShopItem> shopItems = new List<ShopItem>(ShopItems);

        foreach (Transform pos in ShopPositions)
        {
            while(!ItemChosenAndShowen(ref shopItems, pos)){
                
            }
            Debug.Log("Found suitable object");
        }
    }


    bool ItemChosenAndShowen(ref List<ShopItem> shopItems, Transform pos)
    {

        int randomIndex = Random.Range(0, shopItems.Count);

        var bulletInventory = PlayerShoot.Instance.gameObject.GetComponent<BulletInventory>();

        if (shopItems[randomIndex].BulletType != null) //jestli je na to potreba bullettype tak musi byt enabled jinak se to neukaze
        {
            if (!bulletInventory.bulletsEnabled.Contains(shopItems[randomIndex].BulletType))
            {
                shopItems.RemoveAt(randomIndex);
                return false;
            }
        }

        if (randomIndex >= shopItems.Count)
        {
            return false;

        }

        if (shopItems[randomIndex].DontShowThisProductAnymore == true)
        {
            return false;

        }

        GameObject shopItem = Instantiate(shopImageShow, pos.position, Quaternion.identity);
        shopItem.GetComponent<ShopWorldItem>().representedItem = (ShopItem)shopItems[randomIndex];
        shopItem.GetComponent<ShopWorldItem>().shopManager = this;

        shopItems.RemoveAt(randomIndex);
        Debug.Log($"Added {shopItem.GetComponent<ShopWorldItem>().representedItem.itemName} to shop");
        return true;
    }

}

