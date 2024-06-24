using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    // 플레이어 인벤토리 UI의 슬롯
    public class ItemInventorySlot : MonoBehaviour {

        [SerializeField]
        PlayerInventoryManager playerInventory;
        [SerializeField]
        PlayerWeaponSlotManager weaponSlotManager;
        [SerializeField]
        UIManager uiManager;

        public ItemInfoWindowUI itemInfoWindowUI;

        // 슬롯은 아이콘과 아이템을 갖는다.
        public Image icon;
        //WeaponItem item;
        [SerializeField]
        Item item;

        private void Awake() {
            playerInventory = FindObjectOfType<PlayerInventoryManager>();
            weaponSlotManager = FindObjectOfType<PlayerWeaponSlotManager>();
            uiManager = FindObjectOfType<UIManager>();
        }

        // 슬롯에 아이템 추가
        public void AddItem(Item newItem) {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        // 슬롯에서 무기 제거
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
            // 만약 오른쪽손 슬롯1을 선택하여 플레이어 무기 인벤토리로 들어갔고,
            if (uiManager.rightHandSlot1Selected) {
                // 오른쪽손 슬롯1이 비어있지 않다면,
                if (playerInventory.weaponsInRightHandSlots[0] != null) {
                    // 무기 인벤토리 리스트에 현재 오른쪽손 슬롯1에 장착된 무기를 추가
                    playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
                    // 인벤토리 슬롯의 item과 교체를 위해 기억
                    memorizedWeapon = playerInventory.weaponsInRightHandSlots[0];
                }
                // 오른쪽손 슬롯1에 이 슬롯이 가지고있는 item을 전달
                playerInventory.weaponsInRightHandSlots[0] = item as WeaponItem;
                // 무기 인벤토리 리스트에서 해당 item을 제거
                playerInventory.weaponsInventory.Remove(item as WeaponItem);
                // 만약 오른쪽손 슬롯1이 빈 슬롯이었다면 인벤토리 슬롯에 아무 아이템도 들어있지 않게되므로 인벤토리 슬롯을 비움
                if (memorizedWeapon == null) ClearInventorySlot();
                if (playerInventory.currentRightWeaponIndex == 0) changeNow = true; // 바로 바꿔줘야함
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

            // 무기를 교체했으므로 새로 로드한다.
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
