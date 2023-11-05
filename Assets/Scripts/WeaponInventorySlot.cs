using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sg {
    public class WeaponInventorySlot : MonoBehaviour {
        // ������ �����ܰ� �������� ���´�.
        public Image icon;
        WeaponItem item;

        // ���Կ� ���� �߰�
        public void AddItem(WeaponItem newItem) {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        // ���Կ��� ���� ����
        public void ClearInventorySlot() {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
