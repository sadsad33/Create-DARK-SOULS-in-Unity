using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    public class ItemInfoWindowUI : MonoBehaviour {
        public Image itemImage;
        public Text itemText;

        public void SetItemInfo(WeaponItem weaponItem) {
            itemImage.sprite = weaponItem.itemIcon;
            itemText.text = weaponItem.flavorText;
        }
    }
}