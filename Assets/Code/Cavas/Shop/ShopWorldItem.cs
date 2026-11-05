using Assets.Code.Cavas;
using UnityEngine;

public class ShopWorldItem : MonoBehaviour
{
    public ShopItem representedItem;
    public ShopManager shopManager;

    public bool playerInRange = false;

    private void Update()
    {
        GetComponent<SpriteRenderer>().sprite = representedItem.itemSprite;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (shopManager != null)
            {
                CanvasShopControl.Instance.UpdateShopCanvas(representedItem, true);
                Debug.Log($"Player in range of {representedItem.itemName}");
                playerInRange = true;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (shopManager != null)
            {
                CanvasShopControl.Instance.UpdateShopCanvas(representedItem, false);
                Debug.Log($"Player in range of {representedItem.itemName}");

                playerInRange = false;

            }
        }
    }



}
