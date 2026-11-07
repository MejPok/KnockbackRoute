using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Code.Cavas
{
    public class CanvasShopControl : MonoBehaviour
    {
        public static CanvasShopControl Instance;
        public TextMeshProUGUI shopCanvas;

        private void Start()
        {
            Instance = this;
            shopCanvas.text = "";
        }

        public void UpdateShopCanvas(ShopItem item, bool isSelected)
        {
            if (!isSelected)
            {
                shopCanvas.text = "";
                return;
            }

            if (item != null)
            {
                shopCanvas.text = $"Item: {item.itemName} Price: {item.itemPrice}, Level: {item.itemLevel}/{item.maxItemLevel},  \n {item.itemDescription} \n Press B to Buy";
                if(item.itemLevel >= item.maxItemLevel)
                {
                    shopCanvas.text += "\nMax Level Reached";
                }
            }
            else
            {
                shopCanvas.text = "";
            }
        }

    }
}