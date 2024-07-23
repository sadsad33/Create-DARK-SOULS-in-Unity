using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerInventoryManager : CharacterInventoryManager {

        // UI와 연동될 플레이어의 인벤토리
        public List<WeaponItem> weaponsInventory;
        public List<ConsumableItem> consumablesInventory;

        protected override void Awake() {
            base.Awake();
        }

        protected override void Start() {
            base.Start();
            LoadRingEffect();
        }
        public void ChangeRightWeapon() {
            currentRightWeaponIndex += 1; // 다음인덱스로 넘어간다.
            if (currentRightWeaponIndex >= weaponsInRightHandSlots.Length || weaponsInRightHandSlots[currentRightWeaponIndex] == null) {
                currentRightWeaponIndex = -1;
                rightWeapon = character.characterWeaponSlotManager.unarmedWeapon;
            } else {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            }
            character.characterWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        }

        public void ChangeLeftWeapon() {
            currentLeftWeaponIndex += 1;
            if (currentLeftWeaponIndex >= weaponsInLeftHandSlots.Length || weaponsInLeftHandSlots[currentLeftWeaponIndex] == null) {
                currentLeftWeaponIndex = -1;
                leftWeapon = character.characterWeaponSlotManager.unarmedWeapon;
            } else {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            }
            character.characterWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        public void ChangeSpell() {
            currentSpellIndex += 1;
            if (memorizedSpells[currentSpellIndex] == null || currentSpellIndex >= memorizedSpells.Length) {
                currentSpellIndex = 0;
                currentSpell = memorizedSpells[currentSpellIndex];
            } else {
                currentSpell = memorizedSpells[currentSpellIndex];
            }
            character.characterWeaponSlotManager.LoadSpellOnSlot(currentSpell);
        }

        public void ChangeConsumableItem() {
            currentConsumableIndex += 1;
            if (currentConsumableIndex >= selectedConsumables.Length || selectedConsumables[currentConsumableIndex] == null) {
                currentConsumableIndex = 0;
                currentConsumable = selectedConsumables[currentConsumableIndex];
            } else {
                currentConsumable = selectedConsumables[currentConsumableIndex];
            }
            character.characterWeaponSlotManager.LoadConsumableOnSlot(currentConsumable);
        }

        // 게임 데이터를 세이브하는 메서드에서 장비들을 로드한 후 호출 
        public void LoadRingEffect() {
            if (ringSlot01 != null) ringSlot01.EquipRing(character);
            if (ringSlot02 != null) ringSlot02.EquipRing(character);
            if (ringSlot03 != null) ringSlot03.EquipRing(character);
            if (ringSlot04 != null) ringSlot04.EquipRing(character);
        }
    }
}
