using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class WeaponInventorySlot : MonoBehaviour {
        PlayerInventoryManager playerInventory;
        PlayerWeaponSlotManager weaponSlotManager;
        UIManager uiManager;

        public ItemInfoWindowUI itemInfoWindowUI;

        // ������ �����ܰ� �������� ���´�.
        public Image icon;
        WeaponItem item;
        //Item item;

        private void Awake() {
            playerInventory = FindObjectOfType<PlayerInventoryManager>();
            weaponSlotManager = FindObjectOfType<PlayerWeaponSlotManager>();
            uiManager = FindObjectOfType<UIManager>();
        }

        // ���Կ� ������ �߰�
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
        public WeaponItem memorizedWeapon;
        ConsumableItem memorizedConsumable;

        public void EquipThisItem() {
            //if (item is WeaponItem)
            EquipWeaponItem();
            //else if (item is ConsumableItem) EquipConsumableItem();
        }

        public void EquipWeaponItem() {
            //WeaponItem weapon = item as WeaponItem;
            // ���� �����ʼ� ����1�� �����Ͽ� �÷��̾� ���� �κ��丮�� ����,
            if (uiManager.rightHandSlot1Selected) {
                // �����ʼ� ����1�� ������� �ʴٸ�,
                if (playerInventory.weaponsInRightHandSlots[0] != null) {
                    // ���� �κ��丮 �迭�� ���� �����ʼ� ����1�� ������ ���⸦ �߰�
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
                    // �κ��丮 ������ item�� ��ü�� ���� ���
                    memorizedWeapon = playerInventory.weaponsInRightHandSlots[0];
                }
                // �����ʼ� ����1�� �� ������ �������ִ� item�� ����
                playerInventory.weaponsInRightHandSlots[0] = item;
                // ���� �κ��丮 �迭���� �ش� item�� ����
                playerInventory.weaponsInventory.Remove(item);
                // ���� �����ʼ� ����1�� �� �����̾��ٸ� �κ��丮 ���Կ� �ƹ� �����۵� ������� �ʰԵǹǷ� �κ��丮 ������ ���
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentRightWeaponIndex == 0) changeNow = true; // �ٷ� �ٲ������
            } else if (uiManager.rightHandSlot2Selected) {
                if (playerInventory.weaponsInRightHandSlots[1] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
                    memorizedWeapon = playerInventory.weaponsInRightHandSlots[1];
                }
                playerInventory.weaponsInRightHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentRightWeaponIndex == 1) changeNow = true;
            } else if (uiManager.rightHandSlot3Selected) {
                if (playerInventory.weaponsInRightHandSlots[2] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[2]);
                    memorizedWeapon = playerInventory.weaponsInRightHandSlots[2];
                }
                playerInventory.weaponsInRightHandSlots[2] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentRightWeaponIndex == 2) changeNow = true;
            } else if (uiManager.leftHandSlot1Selected) {
                if (playerInventory.weaponsInLeftHandSlots[0] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
                    memorizedWeapon = playerInventory.weaponsInLeftHandSlots[0];
                }
                playerInventory.weaponsInLeftHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentLeftWeaponIndex == 0) changeNow = true;
            } else if (uiManager.leftHandSlot2Selected) {
                if (playerInventory.weaponsInLeftHandSlots[1] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
                    memorizedWeapon = playerInventory.weaponsInLeftHandSlots[1];
                }
                playerInventory.weaponsInLeftHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentLeftWeaponIndex == 1) changeNow = true;
            } else if (uiManager.leftHandSlot3Selected) {
                if (playerInventory.weaponsInLeftHandSlots[2] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[2]);
                    memorizedWeapon = playerInventory.weaponsInLeftHandSlots[2];
                }
                playerInventory.weaponsInLeftHandSlots[2] = item;
                playerInventory.weaponsInventory.Remove(item);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentLeftWeaponIndex == 2) changeNow = true;
            } else {
                return;
            }

            Debug.Log(memorizedWeapon);
            if (memorizedWeapon != null) {
                item = memorizedWeapon;
                icon.sprite = memorizedWeapon.itemIcon;
                memorizedWeapon = null;
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
        }

        public void EquipConsumableItem() {

        }

        public void OnClickButton() {
            if (uiManager.handSlotIsSelected) uiManager.CloseWindow();
            else ShowItemInfo();
            uiManager.ResetAllSelectedSlots();
        }

        public void ShowItemInfo() {
            Debug.Log(item);
            uiManager.OpenSelectedWindow(3);
            itemInfoWindowUI.SetItemInfo(item);
        }

    }
}
