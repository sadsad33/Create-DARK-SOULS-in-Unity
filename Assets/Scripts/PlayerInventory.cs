using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerInventory : MonoBehaviour {
        WeaponSlotManager weaponSlotManager;
        public WeaponItem rightWeapon, leftWeapon, unarmedWeapon;

        public WeaponItem[] weaponsInRightHandSlot = new WeaponItem[1]; // ������ ���⽽��
        public WeaponItem[] weaponsInLeftHandSlot = new WeaponItem[1]; // �޼� ���⽽��

        public int currentRightWeaponIndex = -1;
        public int currentLeftWeaponIndex = -1;
        private void Awake() {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start() {
            //rightWeapon = weaponsInRightHandSlot[currentRightWeaponIndex];
            //leftWeapon = weaponsInLeftHandSlot[currentLeftWeaponIndex];
            //weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            //weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            currentRightWeaponIndex = -1;
            currentLeftWeaponIndex = -1; 
            rightWeapon = unarmedWeapon;
            leftWeapon = unarmedWeapon;
            Debug.Log(currentRightWeaponIndex + "," + currentLeftWeaponIndex);
        }

        public void ChangeRightWeapon() {
            currentRightWeaponIndex = currentRightWeaponIndex + 1; // �����ε����� �Ѿ��.

            // ���� �ε����� 0�� ���
            if (currentRightWeaponIndex == 0 && weaponsInRightHandSlot[0] != null) {
                rightWeapon = weaponsInRightHandSlot[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else if (currentRightWeaponIndex == 0 && weaponsInRightHandSlot[0] == null) {
                currentRightWeaponIndex += 1;
            }

            // ���� �ε����� 1�� ���
            else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlot[1] != null) {
                rightWeapon = weaponsInRightHandSlot[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else {
                currentRightWeaponIndex += 1;
            }

            // �迭�� �ε����� ������ ����� �������� �Ѵ�.
            if (currentRightWeaponIndex > weaponsInRightHandSlot.Length) {
                currentRightWeaponIndex = -1;
                rightWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            }
        }

        public void ChangeLeftWeapon() {
            currentLeftWeaponIndex += 1;

            if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlot[0] != null) {
                leftWeapon = weaponsInLeftHandSlot[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            } else if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlot[0] == null) {
                currentLeftWeaponIndex += 1;
            } else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlot[1] != null) {
                leftWeapon = weaponsInLeftHandSlot[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            } else {
                currentLeftWeaponIndex += 1;
            }

            if (currentLeftWeaponIndex > weaponsInLeftHandSlot.Length) {
                currentLeftWeaponIndex = -1;
                leftWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            }
        }

    }
}
