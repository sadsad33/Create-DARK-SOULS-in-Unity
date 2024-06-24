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

        // � ������ �����ؼ� �κ��丮 â�� ���Դ��� ������ �� �ֵ���
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
        public GameObject itemInventorySlotPrefab; // ���� prefab
        public Transform weaponInventorySlotsParent; // ���� �κ��丮�� �θ� ������Ʈ instantiate�� �߰� ������ �����Ҷ� ��� ������ �˱� ����
        public Transform consumableInventorySlotsParent; // �Ҹ�ǰ �κ��丮�� �θ� ������Ʈ
        public ItemInventorySlot[] weaponInventorySlots; // ���� �κ��丮 ���� �迭
        public ItemInventorySlot[] consumableInventorySlots;

        private void Awake() {
            // ������ ���۵Ǹ� Player �� HUD�� UI ������ ���� ���ÿ� ���� ���� �߰�
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

        // UIâ �ϳ��� ���������� ȣ���
        public void UpdateUI() {
            #region Weapon Inventory Slots
            for (int i = 0; i < weaponInventorySlots.Length; i++) {
                if (i < playerInventory.weaponsInventory.Count) {
                    // ���⸦ ������ �κ��丮�� ���Լ��� �����ϴٸ�
                    if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count) {
                        Instantiate(itemInventorySlotPrefab, weaponInventorySlotsParent); // ���� �߰�
                        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<ItemInventorySlot>();
                    }
                    if (playerInventory.weaponsInventory[i] != null) {
                        weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
                    }
                } else { // �ʿ���� ���� ����.
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
            Debug.Log("���� ���� â : " + uiStack.Peek());
            uiStack.Peek().SetActive(false); // ���� ���� �����ִ� â�� �ݴ´�
            uiStack.Pop();
            uiStack.Peek().SetActive(true); // �ٷ� ���� â�� �ٽ� ǥ��
            Debug.Log("���� �ֻ����� ǥ�õǴ� â : " + uiStack.Peek());
        }

        // ������ ���õƴ� ���â�� ������ �ʱ�ȭ�Ѵ�.
        public void ResetAllSelectedEquipmentSlots() {
            Debug.Log("��� ���� ���û��� ����");
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
