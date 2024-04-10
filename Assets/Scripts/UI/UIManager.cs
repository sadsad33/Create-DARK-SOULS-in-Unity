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

        // � ������ �����ؼ� �κ��丮 â�� ���Դ��� ������ �� �ֵ���
        [Header("Equipment Window Slots Selected")]
        public bool rightHandSlot1Selected;
        public bool rightHandSlot2Selected;
        public bool rightHandSlot3Selected;
        public bool leftHandSlot1Selected;
        public bool leftHandSlot2Selected;
        public bool leftHandSlot3Selected;

        public bool handSlotIsSelected = false;

        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab; // ���� prefab
        public Transform weaponInventorySlotsParent; // �κ��丮�� �θ� ������Ʈ
        WeaponInventorySlot[] weaponInventorySlots; // �κ��丮 ���� �迭

        private void Awake() {
            // ������ ���۵Ǹ� Player �� HUD�� UI ������ ���� ���ÿ� ���� ���� �߰�
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
                    // ���⸦ ������ �κ��丮�� ���Լ��� �����ϴٸ�
                    if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count) {
                        Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent); // ���� �߰�
                        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
                    }
                    weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
                } else { // �ʿ���� ���� ����.
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
            uiStack.Peek().SetActive(false); // ���� ���� �����ִ� â�� �ݴ´�
            uiStack.Pop();
            uiStack.Peek().SetActive(true); // �ٷ� ���� â�� �ٽ� ǥ��
        }

        // ������ ���õƴ� ���â�� ������ �ʱ�ȭ�Ѵ�.
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
