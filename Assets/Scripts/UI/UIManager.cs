using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class UIManager : MonoBehaviour {
        public PlayerInventoryManager playerInventory;
        public PlayerStatsManager playerStatsManager;
        public EquipmentWindowUI equipmentWindowUI;
        private QuickSlots quickSlots;
        public Stack<GameObject> uiStack = new();

        [Header("UI Windows")]
        public GameObject hudWindow;
        public GameObject selectMenuWindow;
        public GameObject equipmentScreenWindow;
        public GameObject inventoryWindow;
        public GameObject itemInfoWindow;
        public GameObject bonfireWindow;
        public GameObject levelUpWindow;

        // 어떤 슬롯을 선택해서 인벤토리 창에 들어왔는지 추적할 수 있도록
        [Header("Equipment Window Weapon Slots Selected")]
        public bool rightHandSlot1Selected;
        public bool rightHandSlot2Selected;
        public bool rightHandSlot3Selected;
        public bool leftHandSlot1Selected;
        public bool leftHandSlot2Selected;
        public bool leftHandSlot3Selected;

        public bool handSlotIsSelected = false;

        [Header("Equipment Window Consumable Slots Selected")]
        public bool consumableSlot1Selected;
        public bool consumableSlot2Selected;
        public bool consumableSlot3Selected;

        public bool consumableSlotSelected = false;

        [Header("Weapon Inventory")]
        public GameObject itemInventorySlotPrefab; // 슬롯 prefab
        public Transform weaponInventorySlotsParent; // 무기 인벤토리의 부모 오브젝트 instantiate로 추가 슬롯을 생성할때 어디에 놓일지 알기 위함
        public Transform consumableInventorySlotsParent; // 소모품 인벤토리의 부모 오브젝트
        public ItemInventorySlot[] weaponInventorySlots; // 무기 인벤토리 슬롯 배열
        public ItemInventorySlot[] consumableInventorySlots;

        private void Awake() {
            // 게임이 시작되면 Player 의 HUD를 UI 관리를 위한 스택에 제일 먼저 추가
            uiStack.Push(hudWindow);
            quickSlots = GetComponentInChildren<QuickSlots>();
        }

        private void Start() {
            weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<ItemInventorySlot>();
            consumableInventorySlots = consumableInventorySlotsParent.GetComponentsInChildren<ItemInventorySlot>();
            equipmentWindowUI.LoadItemsOnEquipmentScreen(playerInventory);
            quickSlots.UpdateCurrentSpellIcon(playerInventory.currentSpell);
            quickSlots.UpdateCurrentConsumableIcon(playerInventory.currentConsumable);
            hudWindow.transform.GetChild(3).GetComponent<SoulCountBar>().SetSoulCountText(playerStatsManager.soulCount);
        }

        // UI창 하나가 열릴때마다 호출됨
        public void UpdateUI() {
            #region Weapon Inventory Slots
            for (int i = 0; i < weaponInventorySlots.Length; i++) {
                if (i < playerInventory.weaponsInventory.Count) {
                    // 무기를 저장할 인벤토리의 슬롯수가 부족하다면
                    if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count) {
                        Instantiate(itemInventorySlotPrefab, weaponInventorySlotsParent); // 슬롯 추가
                        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<ItemInventorySlot>();
                    }
                    if (playerInventory.weaponsInventory[i] != null) {
                        weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
                    }
                } else { // 필요없는 곳은 비운다.
                    weaponInventorySlots[i].ClearInventorySlot();
                }
            }
            #endregion

            #region Consumable Inventory Slots
            for (int i = 0; i < consumableInventorySlots.Length; i++) {
                if (i < playerInventory.consumablesInventory.Count) {
                    if (consumableInventorySlots.Length < playerInventory.consumablesInventory.Count) {
                        Instantiate(itemInventorySlotPrefab, consumableInventorySlotsParent);
                        consumableInventorySlots = consumableInventorySlotsParent.GetComponentsInChildren<ItemInventorySlot>();
                    }
                    if (playerInventory.consumablesInventory[i] != null)
                        consumableInventorySlots[i].AddItem(playerInventory.consumablesInventory[i]);
                } else {
                    consumableInventorySlots[i].ClearInventorySlot();
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
                    inventoryWindow.SetActive(true);
                    inventoryWindow.GetComponent<InventoryWindow>().PrintInventoryWindow();
                    uiStack.Push(inventoryWindow);
                    break;
                case 2:
                    equipmentScreenWindow.SetActive(true);
                    uiStack.Push(equipmentScreenWindow);
                    break;
                case 3:
                    itemInfoWindow.SetActive(true);
                    uiStack.Push(itemInfoWindow);
                    break;
                case 4:
                    bonfireWindow.SetActive(true);
                    uiStack.Push(bonfireWindow);
                    break;
                case 5:
                    levelUpWindow.SetActive(true);
                    uiStack.Push(levelUpWindow);
                    break;
            }
        }

        public void CloseWindow() {
            if (uiStack.Peek() == inventoryWindow) ResetAllSelectedEquipmentSlots();
            Debug.Log("현재 닫을 창 : " + uiStack.Peek());
            uiStack.Peek().SetActive(false); // 가장 위에 열려있던 창을 닫는다
            uiStack.Pop();
            uiStack.Peek().SetActive(true); // 바로 다음 창을 다시 표시
            Debug.Log("현재 최상위에 표시되는 창 : " + uiStack.Peek());
        }

        // 이전에 선택됐던 장비창의 슬롯을 초기화한다.
        public void ResetAllSelectedEquipmentSlots() {
            Debug.Log("모든 슬롯 선택상태 해제");
            rightHandSlot1Selected = false;
            rightHandSlot2Selected = false;
            rightHandSlot3Selected = false;
            leftHandSlot1Selected = false;
            leftHandSlot2Selected = false;
            leftHandSlot3Selected = false;

            handSlotIsSelected = false;

            consumableSlot1Selected = false;
            consumableSlot2Selected = false;
            consumableSlot3Selected = false;

            consumableSlotSelected = false;
        }
    }
}
