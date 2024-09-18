using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    // 플레이어 인벤토리 UI의 슬롯
    public class ItemInventorySlot : MonoBehaviour {
        
        // 슬롯은 아이콘과 아이템을 갖는다.
        [SerializeField] Image icon;
        [SerializeField] Item item;

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
            if (UIManager.instance.rightHandSlot1Selected) {
                // 오른쪽손 슬롯1이 비어있지 않다면,
                if (UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[0] != null) {
                    // 무기 인벤토리 리스트에 현재 오른쪽손 슬롯1에 장착된 무기를 추가
                    UIManager.instance.player.playerInventoryManager.weaponsInventory.Add(UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[0]);
                    // 인벤토리 슬롯의 item과 교체를 위해 기억
                    memorizedWeapon = UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[0];
                }

                // 오른쪽손 슬롯1에 이 슬롯이 가지고있는 item을 전달
                UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[0] = item as WeaponItem;
                // 무기 인벤토리 리스트에서 해당 item을 제거
                UIManager.instance.player.playerInventoryManager.weaponsInventory.Remove(item as WeaponItem);
                // 만약 오른쪽손 슬롯1이 빈 슬롯이었다면 인벤토리 슬롯에 아무 아이템도 들어있지 않게되므로 인벤토리 슬롯을 비움
                if (memorizedWeapon == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentRightWeaponIndex == 0) changeNow = true; // 바로 바꿔줘야함
            } else if (UIManager.instance.rightHandSlot2Selected) {
                if (UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[1] != null) {
                    UIManager.instance.player.playerInventoryManager.weaponsInventory.Add(UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[1]);
                    memorizedWeapon = UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[1];
                }
                UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[1] = item as WeaponItem;
                UIManager.instance.player.playerInventoryManager.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentRightWeaponIndex == 1) changeNow = true;
            } else if (UIManager.instance.rightHandSlot3Selected) {
                if (UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[2] != null) {
                    UIManager.instance.player.playerInventoryManager.weaponsInventory.Add(UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[2]);
                    memorizedWeapon = UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[2];
                }
                UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[2] = item as WeaponItem;
                UIManager.instance.player.playerInventoryManager.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentRightWeaponIndex == 2) changeNow = true;
            } else if (UIManager.instance.leftHandSlot1Selected) {
                if (UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[0] != null) {
                    UIManager.instance.player.playerInventoryManager.weaponsInventory.Add(UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[0]);
                    memorizedWeapon = UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[0];
                }
                UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[0] = item as WeaponItem;
                UIManager.instance.player.playerInventoryManager.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentLeftWeaponIndex == 0) changeNow = true;
            } else if (UIManager.instance.leftHandSlot2Selected) {
                if (UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[1] != null) {
                    UIManager.instance.player.playerInventoryManager.weaponsInventory.Add(UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[1]);
                    memorizedWeapon = UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[1];
                }
                UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[1] = item as WeaponItem;
                UIManager.instance.player.playerInventoryManager.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentLeftWeaponIndex == 1) changeNow = true;
            } else if (UIManager.instance.leftHandSlot3Selected) {
                if (UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[2] != null) {
                    UIManager.instance.player.playerInventoryManager.weaponsInventory.Add(UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[2]);
                    memorizedWeapon = UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[2];
                }
                UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[2] = item as WeaponItem;
                UIManager.instance.player.playerInventoryManager.weaponsInventory.Remove(item as WeaponItem);
                if (memorizedWeapon == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentLeftWeaponIndex == 2) changeNow = true;
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
                UIManager.instance.player.playerInventoryManager.rightWeapon = UIManager.instance.player.playerInventoryManager.weaponsInRightHandSlots[UIManager.instance.player.playerInventoryManager.currentRightWeaponIndex];
                UIManager.instance.player.playerInventoryManager.leftWeapon = UIManager.instance.player.playerInventoryManager.weaponsInLeftHandSlots[UIManager.instance.player.playerInventoryManager.currentLeftWeaponIndex];
                changeNow = !changeNow;
            }

            // 무기를 교체했으므로 새로 로드한다.
            UIManager.instance.player.playerWeaponSlotManager.LoadWeaponOnSlot(UIManager.instance.player.playerInventoryManager.rightWeapon, false);
            UIManager.instance.player.playerWeaponSlotManager.LoadWeaponOnSlot(UIManager.instance.player.playerInventoryManager.leftWeapon, true);

            UIManager.instance.equipmentWindowUI.LoadItemsOnEquipmentScreen(UIManager.instance.player.playerInventoryManager);
        }

        public void EquipConsumableItem() {
            if (UIManager.instance.consumableSlot1Selected) {
                if (UIManager.instance.player.playerInventoryManager.selectedConsumables[0] != null) {
                    UIManager.instance.player.playerInventoryManager.consumablesInventory.Add(UIManager.instance.player.playerInventoryManager.selectedConsumables[0]);
                    memorizedConsumable = UIManager.instance.player.playerInventoryManager.selectedConsumables[0];
                }

                UIManager.instance.player.playerInventoryManager.selectedConsumables[0] = item as ConsumableItem;
                UIManager.instance.player.playerInventoryManager.consumablesInventory.Remove(item as ConsumableItem);
                if (memorizedConsumable == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentConsumableIndex == 0) changeNow = true;
            } else if (UIManager.instance.consumableSlot2Selected) {
                if (UIManager.instance.player.playerInventoryManager.selectedConsumables[1] != null) {
                    UIManager.instance.player.playerInventoryManager.consumablesInventory.Add(UIManager.instance.player.playerInventoryManager.selectedConsumables[1]);
                    memorizedConsumable = UIManager.instance.player.playerInventoryManager.selectedConsumables[1];
                }
                UIManager.instance.player.playerInventoryManager.selectedConsumables[1] = item as ConsumableItem;
                UIManager.instance.player.playerInventoryManager.consumablesInventory.Remove(item as ConsumableItem);
                if (memorizedConsumable == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentConsumableIndex == 1) changeNow = true;
            } else if (UIManager.instance.consumableSlot3Selected) {
                if (UIManager.instance.player.playerInventoryManager.selectedConsumables[2] != null) {
                    UIManager.instance.player.playerInventoryManager.consumablesInventory.Add(UIManager.instance.player.playerInventoryManager.selectedConsumables[2]);
                    memorizedConsumable = UIManager.instance.player.playerInventoryManager.selectedConsumables[2];
                }
                UIManager.instance.player.playerInventoryManager.selectedConsumables[2] = item as ConsumableItem;
                UIManager.instance.player.playerInventoryManager.consumablesInventory.Remove(item as ConsumableItem);
                if (memorizedConsumable == null) ClearInventorySlot();
                if (UIManager.instance.player.playerInventoryManager.currentConsumableIndex == 2) changeNow = true;
            } else return;

            if (memorizedConsumable != null) {
                item = memorizedConsumable;
                icon.sprite = memorizedConsumable.itemIcon;
                memorizedConsumable = null;
            }

            UIManager.instance.equipmentWindowUI.LoadItemsOnEquipmentScreen(UIManager.instance.player.playerInventoryManager);
        }

        // 아이템 슬롯 클릭 이벤트
        public void OnClickItemInventorySlot() {
            if (UIManager.instance.handSlotIsSelected || UIManager.instance.consumableSlotSelected) UIManager.instance.CloseWindow();
            else ShowItemInfo();
            UIManager.instance.ResetAllSelectedEquipmentSlots();
        }

        public void ShowItemInfo() {
            if (item == null) return;
            UIManager.instance.OpenSelectedWindow(3);
            //itemInfoWindowUI.SetItemInfo(item);
            UIManager.instance.itemInfoWindow.GetComponentInChildren<ItemInfoWindowUI>().SetItemInfo(item);
        }
    }
}
