using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerInventoryManager : CharacterInventoryManager {
        
        // UI와 연동될 플레이어의 인벤토리
        public List<WeaponItem> weaponsInventory; 
        public List<ConsumableItem> consumablesInventory;
        
        public void ChangeRightWeapon() {
            currentRightWeaponIndex += 1; // 다음인덱스로 넘어간다.
            if (currentRightWeaponIndex >= weaponsInRightHandSlots.Length || weaponsInRightHandSlots[currentRightWeaponIndex] == null) {
                currentRightWeaponIndex = -1;
                rightWeapon = characterWeaponSlotManager.unarmedWeapon;
            } else {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            }
            characterWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        }

        public void ChangeLeftWeapon() {
            currentLeftWeaponIndex += 1;
            if (currentLeftWeaponIndex >= weaponsInLeftHandSlots.Length || weaponsInLeftHandSlots[currentLeftWeaponIndex] == null) {
                currentLeftWeaponIndex = -1;
                leftWeapon = characterWeaponSlotManager.unarmedWeapon;
            } else {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            }
            characterWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        public void ChangeSpell() {
            currentSpellIndex += 1;
            if (memorizedSpells[currentSpellIndex] == null || currentSpellIndex >= memorizedSpells.Length) {
                currentSpellIndex = 0;
                currentSpell = memorizedSpells[currentSpellIndex];
            } else {
                currentSpell = memorizedSpells[currentSpellIndex];
            }
            characterWeaponSlotManager.LoadSpellOnSlot(currentSpell);
        }

        public void ChangeConsumableItem() {
            currentConsumableIndex += 1;
            if (selectedConsumables[currentConsumableIndex] == null || currentConsumableIndex >= selectedConsumables.Length) {
                currentConsumableIndex = 0;
                currentConsumable = selectedConsumables[currentConsumableIndex];
            } else {
                currentConsumable = selectedConsumables[currentConsumableIndex];
            }
            characterWeaponSlotManager.LoadConsumableOnSlot(currentConsumable);
        }
    }
}
