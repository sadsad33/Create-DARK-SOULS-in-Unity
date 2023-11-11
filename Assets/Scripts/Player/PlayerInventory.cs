using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerInventory : MonoBehaviour {
        WeaponSlotManager weaponSlotManager;
        public WeaponItem rightWeapon, leftWeapon, unarmedWeapon;

        public SpellItem currentSpell;
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[2]; // ������ ���⽽��
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[2]; // �޼� ���⽽��

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

            // ���� �ε����� 0�� ���
            if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] != null) {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null) {
                currentRightWeaponIndex += 1;
            }

            //���� �ε����� 1�� ���
            else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null) {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else {
                currentRightWeaponIndex += 1;
            }

            // �迭�� �ε����� ������ ����� �������� �Ѵ�.
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
