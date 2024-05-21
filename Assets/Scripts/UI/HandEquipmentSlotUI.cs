using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    // �÷��̾� ���â UI�� ����
    public class HandEquipmentSlotUI : MonoBehaviour {

        UIManager uiManager;
        public Image icon;
        Item item;

        public bool rightHandSlot1;
        public bool rightHandSlot2;
        public bool rightHandSlot3;
        public bool leftHandSlot1;
        public bool leftHandSlot2;
        public bool leftHandSlot3;

        public bool consumableSlot1;
        public bool consumableSlot2;
        public bool consumableSlot3;

        private void Awake() {
            uiManager = FindObjectOfType<UIManager>();
        }

        // ���Կ� ������ �߰�
        public void AddItem(Item newItem) {
            if (newItem != null) {
                item = newItem;
                icon.sprite = newItem.itemIcon;
                icon.enabled = true;
                gameObject.SetActive(true);
            }
        }

        // ���Կ��� ������ ����
        public void ClearItem() {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void SelectThisHandSlot() {
            if (rightHandSlot1) {
                uiManager.rightHandSlot1Selected = true;
            } else if (rightHandSlot2) {
                uiManager.rightHandSlot2Selected = true;
            } else if (rightHandSlot3) {
                uiManager.rightHandSlot3Selected = true;
            } else if (leftHandSlot1) {
                uiManager.leftHandSlot1Selected = true;
            } else if (leftHandSlot2) {
                uiManager.leftHandSlot2Selected = true;
            } else {
                uiManager.leftHandSlot3Selected = true;
            }

            uiManager.handSlotIsSelected = true;
        }

        public void SelectThisConsumableSlot() {
            if (consumableSlot1) {
                uiManager.consumableSlot1Selected = true;
            } else if (consumableSlot2) {
                uiManager.consumableSlot2Selected = true;
            } else if (consumableSlot3) {
                uiManager.consumableSlot3Selected = true;
            }

            uiManager.consumableSlotSelected = true;
        }
    }
}
