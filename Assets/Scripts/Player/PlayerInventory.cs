using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerInventory : MonoBehaviour {
        WeaponSlotManager weaponSlotManager;
        public WeaponItem rightWeapon, leftWeapon, unarmedWeapon;

        public SpellItem currentSpell;
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[2]; // 오른손 무기슬롯
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[2]; // 왼손 무기슬롯

        public int currentRightWeaponIndex;
        public int currentLeftWeaponIndex;
        public List<WeaponItem> weaponsInventory; // 플레이어의 인벤토리

        private void Awake() {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start() {
            rightWeapon = weaponsInRightHandSlots[0];
            leftWeapon = weaponsInLeftHandSlots[0];
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        public void ChangeRightWeapon() {
            currentRightWeaponIndex += 1; // 다음인덱스로 넘어간다.

            // 다음 인덱스가 0일 경우
            if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] != null) {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null) {
                currentRightWeaponIndex += 1;
            }

            //다음 인덱스가 1일 경우
            else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null) {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else {
                currentRightWeaponIndex += 1;
            }

            // 배열의 인덱스가 범위를 벗어나면 무장해제 한다.
            if (currentRightWeaponIndex >= weaponsInRightHandSlots.Length) {
                currentRightWeaponIndex = -1;
                rightWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            }
        }

        public void ChangeLeftWeapon() {
            currentLeftWeaponIndex += 1;

            if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] != null) {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            } else if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] == null) {
                currentLeftWeaponIndex += 1;
            } else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] != null) {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            } else {
                currentLeftWeaponIndex += 1;
            }

            if (currentLeftWeaponIndex >= weaponsInLeftHandSlots.Length) {
                currentLeftWeaponIndex = -1;
                leftWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            }
        }

    }
}
