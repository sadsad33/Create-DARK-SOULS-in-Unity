using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class WeaponInventorySlot : MonoBehaviour {
        PlayerInventoryManager playerInventory;
        PlayerWeaponSlotManager weaponSlotManager;
        UIManager uiManager; 

        // 슬롯은 아이콘과 아이템을 갖는다.
        public Image icon;
        WeaponItem item;

        private void Awake() {
            playerInventory = FindObjectOfType<PlayerInventoryManager>();
            weaponSlotManager = FindObjectOfType<PlayerWeaponSlotManager>();
            uiManager = FindObjectOfType<UIManager>();
        }
        // 슬롯에 무기 추가
        public void AddItem(WeaponItem newItem) {
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
        public void EquipThisItem() {
            // 만약 오른쪽손 슬롯1을 선택하여 플레이어 무기 인벤토리로 들어갔다면
            if (uiManager.rightHandSlot1Selected) {
                // 무기 인벤토리에 현재 오른쪽손 슬롯1에 장착된 무기를 추가
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
                // 오른쪽손 슬롯1에는 무기 인벤토리에서 선택된 아이템을 추가
                playerInventory.weaponsInRightHandSlots[0] = item;
                // 무기 인벤토리에서 선택된 아이템은 무기 인벤토리에서 제거
                playerInventory.weaponsInventory.Remove(item);
                // 장비창에서 바꾸려고 선택한 무기 슬롯의 인덱스와 현재 오른쪽 무기의 인덱스가 일치한다면
                if (playerInventory.currentRightWeaponIndex == 0) changeNow = true; // 바로 바꿔줘야함
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

            // 무기를 교체했으므로 새로 로드한다.
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false); 
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

            uiManager.equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
            uiManager.ResetAllSelectedSlots();
        }
    }
}
