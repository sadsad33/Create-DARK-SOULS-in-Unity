using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class EquipmentWindowUI : MonoBehaviour {

        //public bool rightHandSlot1, rightHandSlot2, rightHandSlot3;
        //public bool leftHandSlot1, leftHandSlot2, leftHandSlot3;
        public HandEquipmentSlotUI[] handEquipmentSlotUI;

        //public bool consumableSlot1, consumableSlot2, consumableSlot3;
        public HandEquipmentSlotUI[] consumableEquipmentSlotUI;

        // ��ũ�ҿ� ó�� �����հ� �޼տ� ���� �������� ������� ���ϰ�����
        // ���â�� �÷��̾ ���� ��տ� ����ִ� ������ �Ҹ�ǰ���� ǥ���Ѵ�.
        public void LoadItemsOnEquipmentScreen(PlayerInventoryManager playerInventory) {
            for (int i = 0; i < handEquipmentSlotUI.Length; i++) {
                if (handEquipmentSlotUI[i].rightHandSlot1) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[0]);
                } else if (handEquipmentSlotUI[i].rightHandSlot2) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[1]);
                } else if (handEquipmentSlotUI[i].rightHandSlot3) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[2]);
                } else if (handEquipmentSlotUI[i].leftHandSlot1) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[0]);
                }else if(handEquipmentSlotUI[i].leftHandSlot2){
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[1]);
                } else {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[2]);
                }
            }

            for (int i = 0; i < consumableEquipmentSlotUI.Length; i++) {
                if (consumableEquipmentSlotUI[i].consumableSlot1) {
                    consumableEquipmentSlotUI[i].AddItem(playerInventory.selectedConsumables[0]);
                } else if (consumableEquipmentSlotUI[i].consumableSlot2) {
                    consumableEquipmentSlotUI[i].AddItem(playerInventory.selectedConsumables[1]);
                } else if (consumableEquipmentSlotUI[i].consumableSlot3) {
                    consumableEquipmentSlotUI[i].AddItem(playerInventory.selectedConsumables[2]);
                }
            }
        }

        //public void SelectRightHandSlot1() {
        //    rightHandSlot1 = true;
        //}

        //public void SelectRightHandSlot2() {
        //    rightHandSlot2 = true;
        //}

        //public void SelectRightHandSlot3() {
        //    rightHandSlot3 = true;
        //}

        //public void SelectLeftHandSlot1() {
        //    leftHandSlot1 = true;
        //}

        //public void SelectLeftHandSlot2() {
        //    leftHandSlot2 = true;
        //}

        //public void SelectLeftHandSlot3() {
        //    leftHandSlot3 = true;
        //}

        //public void SelectConsumableSlot1() {
        //    consumableSlot1 = true;
        //}

        //public void SelectConsumableSlot2() {
        //    consumableSlot2 = true;
        //}

        //public void SelectConsumableSlot3() {
        //    consumableSlot3 = true;
        //}
    }
}