using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sg {
    public class UIManager : MonoBehaviour {
        public PlayerInventory playerInventory;
        EquipmentWindowUI equipmentWindowUI;

        [Header("UI Windows")]
        public GameObject hudWindow;
        public GameObject selectWindow;
        public GameObject weaponInventoryWindow;

        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab; // 슬롯 prefab
        public Transform weaponInventorySlotsParent; // 인벤토리의 부모 오브젝트
        WeaponInventorySlot[] weaponInventorySlots; // 인벤토리 슬롯 배열

        private void Awake() {
            equipmentWindowUI = FindObjectOfType<EquipmentWindowUI>();
        }
        private void Start() {
            weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
            equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
        }
        public void UpdateUI() {
            #region Weapon Inventory Slots
            for (int i = 0; i < weaponInventorySlots.Length; i++) {
                if (i < playerInventory.weaponsInventory.Count) {
                    // 무기를 저장할 인벤토리의 슬롯수가 부족하다면
                    if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count) {
                        Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent); // 슬롯 추가
                        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
                    }
                    weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
                } else { // 필요없는 곳은 비운다.
                    weaponInventorySlots[i].ClearInventorySlot();
                }
            }

            #endregion
        }
        public void OpenSelectWindow() {
            selectWindow.SetActive(true);
        }

        public void CloseSelectWindow() {
            selectWindow.SetActive(false);
        }

        public void CloseAllInventoryWindows() {
            weaponInventoryWindow.SetActive(false);
        }
    }
}
