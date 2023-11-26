using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerInventory : MonoBehaviour {
        WeaponSlotManager weaponSlotManager;
        public WeaponItem rightWeapon, leftWeapon, unarmedWeapon;
        public SpellItem currentSpell;
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3]; // ������ ���⽽��
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3]; // �޼� ���⽽��

        public int currentRightWeaponIndex;
        public int currentLeftWeaponIndex;
        public List<WeaponItem> weaponsInventory; // �÷��̾��� �κ��丮

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
            currentRightWeaponIndex += 1; // �����ε����� �Ѿ��.

            // �迭�� �ε����� ������ ����� �������� �Ѵ�.
            if (currentRightWeaponIndex >= weaponsInRightHandSlots.Length) {
                currentRightWeaponIndex = -1;
                rightWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else {
                if (weaponsInRightHandSlots[currentRightWeaponIndex] != null) {
                    rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                    weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
                } else {
                    currentRightWeaponIndex += 1;
                }
            }
        }

        public void ChangeLeftWeapon() {
            currentLeftWeaponIndex += 1;

            if (currentLeftWeaponIndex >= weaponsInLeftHandSlots.Length) {
                currentLeftWeaponIndex = -1;
                leftWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            } else {
                if (weaponsInLeftHandSlots[currentLeftWeaponIndex] != null) {
                    leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                    weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
                } else {
                    currentLeftWeaponIndex += 1;
                }
            }
        }

    }
}
