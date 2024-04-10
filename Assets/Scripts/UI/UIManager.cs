using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class UIManager : MonoBehaviour {
        public PlayerInventoryManager playerInventory;
        public EquipmentWindowUI equipmentWindowUI;
        private QuickSlots quickSlots;
        public Stack<GameObject> uiStack = new();

        [Header("UI Windows")]
        public GameObject hudWindow;
        public GameObject selectMenuWindow;
        public GameObject equipmentScreenWindow;
        public GameObject weaponInventoryWindow;
        public GameObject ItemInfoWindow;

        // 어떤 슬롯을 선택해서 인벤토리 창에 들어왔는지 추적할 수 있도록
        [Header("Equipment Window Slots Selected")]
        public bool rightHandSlot1Selected;
        public bool rightHandSlot2Selected;
        public bool rightHandSlot3Selected;
        public bool leftHandSlot1Selected;
        public bool leftHandSlot2Selected;
        public bool leftHandSlot3Selected;

        public bool handSlotIsSelected = false;

        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab; // 슬롯 prefab
        public Transform weaponInventorySlotsParent; // 인벤토리의 부모 오브젝트
        WeaponInventorySlot[] weaponInventorySlots; // 인벤토리 슬롯 배열

        private void Awake() {
            // 게임이 시작되면 Player 의 HUD를 UI 관리를 위한 스택에 제일 먼저 추가
            uiStack.Push(hudWindow);
            quickSlots = GetComponentInChildren<QuickSlots>();
        }

        private void Start() {
            weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
            equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
            quickSlots.UpdateCurrentSpellIcon(playerInventory.currentSpell);
            quickSlots.UpdateCurrentConsumableIcon(playerInventory.currentConsumable);
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

        public void OpenSelectedWindow(int code) {
            uiStack.Peek().SetActive(false);
            switch (code) {
                case 0:
                    selectMenuWindow.SetActive(true);
                    uiStack.Push(selectMenuWindow);
                    break;
                case 1:
                    weaponInventoryWindow.SetActive(true);
                    uiStack.Push(weaponInventoryWindow);
                    break;
                case 2:
                    equipmentScreenWindow.SetActive(true);
                    uiStack.Push(equipmentScreenWindow);
                    break;
                case 3:
                    ItemInfoWindow.SetActive(true);
                    uiStack.Push(ItemInfoWindow);
                    break;
            }
        }

        public void CloseWindow() {
            uiStack.Peek().SetActive(false); // 가장 위에 열려있던 창을 닫는다
            uiStack.Pop();
            uiStack.Peek().SetActive(true); // 바로 다음 창을 다시 표시
        }

        // 이전에 선택됐던 장비창의 슬롯을 초기화한다.
        public void ResetAllSelectedSlots() {
            rightHandSlot1Selected = false;
            rightHandSlot2Selected = false;
            rightHandSlot3Selected = false;
            leftHandSlot1Selected = false;
            leftHandSlot2Selected = false;
            leftHandSlot3Selected = false;

            handSlotIsSelected = false;
        }
    }
}
