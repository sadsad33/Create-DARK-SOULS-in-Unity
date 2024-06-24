using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    // �÷��̾� �κ��丮 UI�� ����
    public class ItemInventorySlot : MonoBehaviour {

        [SerializeField]
        PlayerInventoryManager playerInventory;
        [SerializeField]
        PlayerWeaponSlotManager weaponSlotManager;
        [SerializeField]
        UIManager uiManager;

        public ItemInfoWindowUI itemInfoWindowUI;

        // ������ �����ܰ� �������� ���´�.
        public Image icon;
        //WeaponItem item;
        [SerializeField]
        Item item;

        private void Awake() {
            playerInventory = FindObjectOfType<PlayerInventoryManager>();
            weaponSlotManager = FindObjectOfType<PlayerWeaponSlotManager>();
            uiManager = FindObjectOfType<UIManager>();
        }

        // ���Կ� ������ �߰�
        public void AddItem(Item newItem) {
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
        WeaponItem memorizedWeapon;
        ConsumableItem memorizedConsumable;

        public void EquipThisItem() {
            if (item is WeaponItem) EquipWeaponItem();
            else if (item is ConsumableItem) EquipConsumableItem();
        }

        public void EquipWeaponItem() {
            // ���� �����ʼ� ����1�� �����Ͽ� �÷��̾� ���� �κ��丮�� ����,
            if (uiManager.rightHandSlot1Selected) {
                // �����ʼ� ����1�� ������� �ʴٸ�,
                if (playerInventory.weaponsInRightHandSlots[0] != null) {
                    // ���� �κ��丮 ����Ʈ�� ���� �����ʼ� ����1�� ������ ���⸦ �߰�
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
                    // �κ��丮 ������ item�� ��ü�� ���� ���
                    memorizedWeapon = playerInventory.weaponsInRightHandSlots[0];
                }
                // �����ʼ� ����1�� �� ������ �������ִ� item�� ����
                playerInventory.weaponsInRightHandSlots[0] = item as WeaponItem;
                // ���� �κ��丮 ����Ʈ���� �ش� item�� ����
                playerInventory.weaponsInventory.Remove(item as WeaponItem);
                // ���� �����ʼ� ����1�� �� �����̾��ٸ� �κ��丮 ���Կ� �ƹ� �����۵� ������� �ʰԵǹǷ� �κ��丮 ������ ���
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentRightWeaponIndex == 0) changeNow = true; // �ٷ� �ٲ������
            } else if (uiManager.rightHandSlot2Selected) {
                if (playerInventory.weaponsInRightHandSlots[1] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
                    memorizedWeapon = playerInventory.weaponsInRightHandSlots[1];
                }
                playerInventory.weaponsInRightHandSlots[1] = item as WeaponItem;
                playerInventory.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentRightWeaponIndex == 1) changeNow = true;
            } else if (uiManager.rightHandSlot3Selected) {
                if (playerInventory.weaponsInRightHandSlots[2] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[2]);
                    memorizedWeapon = playerInventory.weaponsInRightHandSlots[2];
                }
                playerInventory.weaponsInRightHandSlots[2] = item as WeaponItem;
                playerInventory.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentRightWeaponIndex == 2) changeNow = true;
            } else if (uiManager.leftHandSlot1Selected) {
                if (playerInventory.weaponsInLeftHandSlots[0] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
                    memorizedWeapon = playerInventory.weaponsInLeftHandSlots[0];
                }
                playerInventory.weaponsInLeftHandSlots[0] = item as WeaponItem;
                playerInventory.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentLeftWeaponIndex == 0) changeNow = true;
            } else if (uiManager.leftHandSlot2Selected) {
                if (playerInventory.weaponsInLeftHandSlots[1] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
                    memorizedWeapon = playerInventory.weaponsInLeftHandSlots[1];
                }
                playerInventory.weaponsInLeftHandSlots[1] = item as WeaponItem;
                playerInventory.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentLeftWeaponIndex == 1) changeNow = true;
            } else if (uiManager.leftHandSlot3Selected) {
                if (playerInventory.weaponsInLeftHandSlots[2] != null) {
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[2]);
                    memorizedWeapon = playerInventory.weaponsInLeftHandSlots[2];
                }
                playerInventory.weaponsInLeftHandSlots[2] = item as WeaponItem;
                playerInventory.weaponsInventory.Remove(item as WeaponItem);
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

            uiManager.equipmentWindowUI.LoadItemsOnEquipmentScreen(playerInventory);
        }

        public void EquipConsumableItem() {
            if (uiManager.consumableSlot1Selected) {
                if (playerInventory.selectedConsumables[0] != null) {
                    playerInventory.consumablesInventory.Add(playerInventory.selectedConsumables[0]);
                    memorizedConsumable = playerInventory.selectedConsumables[0];
                }

                playerInventory.selectedConsumables[0] = item as ConsumableItem;
                playerInventory.consumablesInventory.Remove(item as ConsumableItem);
                if (memorizedConsumable == null) ClearInventorySlot();
                if (playerInventory.currentConsumableIndex == 0) changeNow = true;
            } else if (uiManager.consumableSlot2Selected) {
                if (playerInventory.selectedConsumables[1] != null) {
                    playerInventory.consumablesInventory.Add(playerInventory.selectedConsumables[1]);
                    memorizedConsumable = playerInventory.selectedConsumables[1];
                }
                playerInventory.selectedConsumables[1] = item as ConsumableItem;
                playerInventory.consumablesInventory.Remove(item as ConsumableItem);
                if (memorizedConsumable == null) ClearInventorySlot();
                if (playerInventory.currentConsumableIndex == 1) changeNow = true;
            } else if (uiManager.consumableSlot3Selected) {
                if (playerInventory.selectedConsumables[2] != null) {
                    playerInventory.consumablesInventory.Add(playerInventory.selectedConsumables[2]);
                    memorizedConsumable = playerInventory.selectedConsumables[2];
                }
                playerInventory.selectedConsumables[2] = item as ConsumableItem;
                playerInventory.consumablesInventory.Remove(item as ConsumableItem);
                if (memorizedConsumable == null) ClearInventorySlot();
                if (playerInventory.currentConsumableIndex == 2) changeNow = true;
            } else return;

            if (memorizedConsumable != null) {
                item = memorizedConsumable;
                icon.sprite = memorizedConsumable.itemIcon;
                memorizedConsumable = null;
            }

            uiManager.equipmentWindowUI.LoadItemsOnEquipmentScreen(playerInventory);
        }

        public void OnClickButton() {
            if (uiManager.handSlotIsSelected || uiManager.consumableSlotSelected) uiManager.CloseWindow();
            else ShowItemInfo();
            uiManager.ResetAllSelectedEquipmentSlots();
        }

        public void ShowItemInfo() {
            Debug.Log(item);
            uiManager.OpenSelectedWindow(3);
            itemInfoWindowUI.SetItemInfo(item);
        }

    }
}
