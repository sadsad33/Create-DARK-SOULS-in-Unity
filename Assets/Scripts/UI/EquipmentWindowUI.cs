using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class EquipmentWindowUI : MonoBehaviour {

        public HandEquipmentSlotUI[] handEquipmentSlotUI;
        public HandEquipmentSlotUI[] consumableEquipmentSlotUI;

        // 다크소울 처럼 오른손과 왼손에 각각 여러개의 무기들을 지니고있음
        // 장비창에 플레이어가 현재 양손에 들고있는 무기들과 소모품들을 표시한다.
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
    }
}