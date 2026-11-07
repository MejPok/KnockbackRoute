using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.Cavas.Shop
{
    class UpgradesManager : MonoBehaviour
    {
        public static UpgradesManager Instance;

        public List<ShopItem> everyShopItem;

        private void Start()
        {
            Instance = this;
        }

        public ShopItem FindRightItem(string itemName)
        {
            return everyShopItem.FirstOrDefault(item => item.name == itemName);
        }
    }
}
