using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    public class ItemInfoWindowUI : MonoBehaviour {
        public Image itemImage;
        public Text itemText;

        public void SetItemInfo(Item item) {
            itemImage.sprite = item.itemIcon;
            itemText.text = item.flavorText;
        }
    }
}