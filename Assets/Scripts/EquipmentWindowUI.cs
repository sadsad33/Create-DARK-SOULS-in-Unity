using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EquipmentWindowUI : MonoBehaviour {
        public bool rightHandSlot1, rightHandSlot2;
        public bool leftHandSlot1, leftHandSlot2;
        public HandEquipmentSlotUI[] handEquipmentSlotUI;
        private void Start() {
        }

        // ��ũ�ҿ� ó�� �����հ� �޼տ� ���� �������� ������� ���ϰ�����
        // ���â�� �÷��̾ ���� ��տ� ����ִ� ������� ǥ���Ѵ�.
        public void LoadWeaponOnEquipmentScreen(PlayerInventory playerInventory) {
            for (int i = 0; i < handEquipmentSlotUI.Length; i++) {
                if (handEquipmentSlotUI[i].rightHandSlot1) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[0]);
                } else if (handEquipmentSlotUI[i].rightHandSlot2) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[1]);
                } else if (handEquipmentSlotUI[i].leftHandSlot1) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[0]);
                } else {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[1]);
                }
            }
        }
        public void SelectRightHandSlot1() {
            rightHandSlot1 = true;
        }

        public void SelectRightHandSlot2() {
            rightHandSlot2 = true;
        }

        public void SelectLeftHandSlot1() {
            leftHandSlot1 = true;
        }

        public void SelectLeftHandSlot2() {
            leftHandSlot2 = true;
        }
    }
}