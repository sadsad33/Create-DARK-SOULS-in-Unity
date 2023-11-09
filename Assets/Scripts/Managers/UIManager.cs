using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sg {
    public class UIManager : MonoBehaviour {
        public PlayerInventory playerInventory;
        public EquipmentWindowUI equipmentWindowUI;

        [Header("UI Windows")]
        public GameObject hudWindow;
        public GameObject selectWindow;
        public GameObject equipmentScreenWindow;
        public GameObject weaponInventoryWindow;

        // 어떤 슬롯을 선택해서 인벤토리 창에 들어왔는지 추적할 수 있도록
        [Header("Equipment Window Slots Selected")]
        public bool rightHandSlot1Selected;
        public bool rightHandSlot2Selected;
        public bool leftHandSlot1Selected; 
        public bool leftHandSlot2Selected;

        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab; // 슬롯 prefab
        public Transform weaponInventorySlotsParent; // 인벤토리의 부모 오브젝트
        WeaponInventorySlot[] weaponInventorySlots; // 인벤토리 슬롯 배열

        private void Awake() {
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
            ResetAllSelectedSlots();
            weaponInventoryWindow.SetActive(false);
            equipmentScreenWindow.SetActive(false);
        }

        // 이전에 선택됐던 장비창의 슬롯을 초기화한다.
        public void ResetAllSelectedSlots() {
            rightHandSlot1Selected = false;
            rightHandSlot2Selected = false;
            leftHandSlot1Selected = false;
            leftHandSlot2Selected = false;
        }
    }
}
