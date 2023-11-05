using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sg {
    public class WeaponInventorySlot : MonoBehaviour {
        // 슬롯은 아이콘과 아이템을 갖는다.
        public Image icon;
        WeaponItem item;

        // 슬롯에 무기 추가
        public void AddItem(WeaponItem newItem) {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        // 슬롯에서 무기 제거
        public void ClearInventorySlot() {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
