using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace sg {
    public class HandEquipmentSlotUI : MonoBehaviour {
        public Image icon;
        WeaponItem weapon;

        public bool rightHandSlot1;
        public bool rightHandSlot2;
        public bool leftHandSlot1;
        public bool leftHandSlot2;

        // ���Կ� ������ �߰�
        public void AddItem(WeaponItem newWeapon) {
            weapon = newWeapon;
            icon.sprite = newWeapon.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        // ���Կ��� ������ ����
        public void ClearItem() {
            weapon = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
