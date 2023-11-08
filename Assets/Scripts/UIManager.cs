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

        // � ������ �����ؼ� �κ��丮 â�� ���Դ��� ������ �� �ֵ���
        [Header("Equipment Window Slots Selected")]
        public bool rightHandSlot1Selected;
        public bool rightHandSlot2Selected;
        public bool leftHandSlot1Selected; 
        public bool leftHandSlot2Selected;

        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab; // ���� prefab
        public Transform weaponInventorySlotsParent; // �κ��丮�� �θ� ������Ʈ
        WeaponInventorySlot[] weaponInventorySlots; // �κ��丮 ���� �迭

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

        // ������ ���õƴ� ���â�� ������ �ʱ�ȭ�Ѵ�.
        public void ResetAllSelectedSlots() {
            rightHandSlot1Selected = false;
            rightHandSlot2Selected = false;
            leftHandSlot1Selected = false;
            leftHandSlot2Selected = false;
        }
    }
}
