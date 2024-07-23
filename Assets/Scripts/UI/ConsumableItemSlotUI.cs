using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class ConsumableItemSlotUI : MonoBehaviour {
        UIManager uiManager;
        public Image icon;
        ConsumableItem consumable;

        public bool slot1;
        public bool slot2;
        public bool slot3;

        private void Awake() {
            uiManager = FindObjectOfType<UIManager>();
        }

        public void AddItem(ConsumableItem newConsumable) {
            if (newConsumable != null) {
                consumable = newConsumable;
                icon.sprite = newConsumable.itemIcon;
                icon.enabled = true;
                gameObject.SetActive(true);
            }
        }

        public void ClearItem() {
            consumable = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void SelectThisSlot() {
            if (slot1) {
                uiManager.consumableSlot1Selected = true;
            } else if (slot2) {
                uiManager.consumableSlot2Selected = true;
            } else if (slot3) {
                uiManager.consumableSlot3Selected = true;
            }

            uiManager.consumableSlotSelected = true;
        }
    }
}
