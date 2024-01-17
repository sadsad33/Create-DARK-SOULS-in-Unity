using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerInventoryManager : MonoBehaviour {
        PlayerWeaponSlotManager playerWeaponSlotManager;

        [Header("Quick Slot Items")]
        public SpellItem currentSpell;
        public WeaponItem rightWeapon, leftWeapon;
        public ConsumableItem currentConsumable;

        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3]; // �޼� ���⽽��
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3]; // ������ ���⽽��
        
        [Header("Current Equipment")]
        public HelmetEquipment currentHelmetEquipment;
        public TorsoEquipment currentTorsoEquipment;
        public LegEquipment currentLegEquipment;
        public GuntletEquipment currentGuntletEquipment;

        public int currentRightWeaponIndex;
        public int currentLeftWeaponIndex;
        public List<WeaponItem> weaponsInventory; // �÷��̾��� �κ��丮

        private void Awake() {
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
        }

        private void Start() {
            rightWeapon = weaponsInRightHandSlots[0];
            leftWeapon = weaponsInLeftHandSlots[0];
            playerWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            playerWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        public void ChangeRightWeapon() {
            currentRightWeaponIndex += 1; // �����ε����� �Ѿ��.
            // �迭�� �ε����� ������ ����� �������� �Ѵ�.
            if (currentRightWeaponIndex >= weaponsInRightHandSlots.Length) {
                currentRightWeaponIndex = -1;
                rightWeapon = playerWeaponSlotManager.unarmedWeapon;
                playerWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            } else {
                if (weaponsInRightHandSlots[currentRightWeaponIndex] != null) {
                    rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                    playerWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
                } else {
                    currentRightWeaponIndex += 1;
                }
            }
        }

        public void ChangeLeftWeapon() {
            currentLeftWeaponIndex += 1;
            if (currentLeftWeaponIndex >= weaponsInLeftHandSlots.Length) {
                currentLeftWeaponIndex = -1;
                leftWeapon = playerWeaponSlotManager.unarmedWeapon;
                playerWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            } else {
                if (weaponsInLeftHandSlots[currentLeftWeaponIndex] != null) {
                    leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                    playerWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
                } else {
                    currentLeftWeaponIndex += 1;
                }
            }
        }

    }
}
