using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace sg {
    public class HandEquipmentSlotUI : MonoBehaviour {

        UIManager uiManager;

        public Image icon;
        WeaponItem weapon;

        public bool rightHandSlot1;
        public bool rightHandSlot2;
        public bool leftHandSlot1;
        public bool leftHandSlot2;

        private void Awake() {
            uiManager = FindObjectOfType<UIManager>();
        }

        // 슬롯에 아이템 추가
        public void AddItem(WeaponItem newWeapon) {
            if (newWeapon != null) {
                weapon = newWeapon;
                icon.sprite = newWeapon.itemIcon;
                icon.enabled = true;
                gameObject.SetActive(true);
            }
        }

        // 슬롯에서 아이템 제거
        public void ClearItem() {
            weapon = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void SelectThisSlot() {
            if (rightHandSlot1) {
                uiManager.rightHandSlot1Selected = true;
            } else if (rightHandSlot2) {
                uiManager.rightHandSlot2Selected = true;
            } else if (leftHandSlot1) {
                uiManager.leftHandSlot1Selected = true;
            } else {
                uiManager.leftHandSlot2Selected = true;
            }
        }
    }
}
