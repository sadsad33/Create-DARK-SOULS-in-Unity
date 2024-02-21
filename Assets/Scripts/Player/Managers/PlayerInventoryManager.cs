using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerInventoryManager : CharacterInventoryManager {
        public List<WeaponItem> weaponsInventory; // �÷��̾��� �κ��丮

        public void ChangeRightWeapon() {
            currentRightWeaponIndex += 1; // �����ε����� �Ѿ��.
            // �迭�� �ε����� ������ ����� �������� �Ѵ�.
            if (currentRightWeaponIndex >= weaponsInRightHandSlots.Length) {
                currentRightWeaponIndex = -1;
                rightWeapon = characterWeaponSlotManager.unarmedWeapon;
                characterWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else {
                if (weaponsInRightHandSlots[currentRightWeaponIndex] != null) {
                    rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                    characterWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
                } else {
                    currentRightWeaponIndex += 1;
                }
            }
        }

        public void ChangeLeftWeapon() {
            currentLeftWeaponIndex += 1;
            if (currentLeftWeaponIndex >= weaponsInLeftHandSlots.Length) {
                currentLeftWeaponIndex = -1;
                leftWeapon = characterWeaponSlotManager.unarmedWeapon;
                characterWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            } else {
                if (weaponsInLeftHandSlots[currentLeftWeaponIndex] != null) {
                    leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                    characterWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
                } else {
                    currentLeftWeaponIndex += 1;
                }
            }
        }

    }
}
