using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class WeaponInventorySlot : MonoBehaviour {
        PlayerInventoryManager playerInventory;
        PlayerWeaponSlotManager weaponSlotManager;
        UIManager uiManager; 

        // ������ �����ܰ� �������� ���´�.
        public Image icon;
        WeaponItem item;

        private void Awake() {
            playerInventory = FindObjectOfType<PlayerInventoryManager>();
            weaponSlotManager = FindObjectOfType<PlayerWeaponSlotManager>();
            uiManager = FindObjectOfType<UIManager>();
        }
        // ���Կ� ���� �߰�
        public void AddItem(WeaponItem newItem) {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        // ���Կ��� ���� ����
        public void ClearInventorySlot() {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        bool changeNow = false;
        public void EquipThisItem() {
            // ���� �����ʼ� ����1�� �����Ͽ� �÷��̾� ���� �κ��丮�� ���ٸ�
            if (uiManager.rightHandSlot1Selected) {
                // ���� �κ��丮�� ���� �����ʼ� ����1�� ������ ���⸦ �߰�
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
                // �����ʼ� ����1���� ���� �κ��丮���� ���õ� �������� �߰�
                playerInventory.weaponsInRightHandSlots[0] = item;
                // ���� �κ��丮���� ���õ� �������� ���� �κ��丮���� ����
                playerInventory.weaponsInventory.Remove(item);
                // ���â���� �ٲٷ��� ������ ���� ������ �ε����� ���� ������ ������ �ε����� ��ġ�Ѵٸ�
                if (playerInventory.currentRightWeaponIndex == 0) changeNow = true; // �ٷ� �ٲ������
            } else if (uiManager.rightHandSlot2Selected) {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
                playerInventory.weaponsInRightHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (playerInventory.currentRightWeaponIndex == 1) changeNow = true;
            } else if (uiManager.rightHandSlot3Selected) {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[2]);
                playerInventory.weaponsInRightHandSlots[2] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (playerInventory.currentRightWeaponIndex == 2) changeNow = true;
            } else if (uiManager.leftHandSlot1Selected) {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
                playerInventory.weaponsInLeftHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (playerInventory.currentLeftWeaponIndex == 0) changeNow = true;
            } else if (uiManager.leftHandSlot2Selected) {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
                playerInventory.weaponsInLeftHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (playerInventory.currentLeftWeaponIndex == 1) changeNow = true;
            } else if (uiManager.leftHandSlot3Selected){
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[2]);
                playerInventory.weaponsInLeftHandSlots[2] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (playerInventory.currentLeftWeaponIndex == 2) changeNow = true;
            } else {
                return;
            }

            if (changeNow) {
                playerInventory.rightWeapon = playerInventory.weaponsInRightHandSlots[playerInventory.currentRightWeaponIndex];
                playerInventory.leftWeapon = playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftWeaponIndex];
                changeNow = !changeNow;
            }

            // ���⸦ ��ü�����Ƿ� ���� �ε��Ѵ�.
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false); 
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

            uiManager.equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
            uiManager.ResetAllSelectedSlots();
        }
    }
}
