using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace sg {
    public class QuickSlots : MonoBehaviour {
        public Image leftWeaponIcon, rightWeaponIcon;
        public Image currentSpellIcon, currentConsumableIcon;

        public void UpdateWeaponQuickSlotsUI(bool isLeft, WeaponItem weapon) {
            if (isLeft == false) {
                if (weapon.itemIcon != null) {
                    rightWeaponIcon.sprite = weapon.itemIcon;
                    rightWeaponIcon.enabled = true;
                } else {
                    rightWeaponIcon.sprite = null;
                    rightWeaponIcon.enabled = false;
                }
            } else {
                if (weapon.itemIcon != null) {
                    leftWeaponIcon.sprite = weapon.itemIcon;
                    leftWeaponIcon.enabled = true;
                } else {
                    leftWeaponIcon.sprite = null;
                    leftWeaponIcon.enabled = false;
                }
            }
        }

        public void UpdateCurrentSpellIcon(SpellItem spellItem) {
            if (spellItem.itemIcon != null) {
                currentSpellIcon.sprite = spellItem.itemIcon;
                currentSpellIcon.enabled = true;
            } else {
                currentSpellIcon.sprite = null;
                currentSpellIcon.enabled = false;
            }
        }

        public void UpdateCurrentConsumableIcon(ConsumableItem consumableItem) {
            if (consumableItem.itemIcon != null) {
                currentConsumableIcon.sprite = consumableItem.itemIcon;
                currentConsumableIcon.enabled = true;
            } else {
                currentConsumableIcon.sprite = null;
                currentConsumableIcon.enabled = false;
            }
        }
    }
}
