using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EquipmentWindowUI : MonoBehaviour {
        public bool rightHandSlot1, rightHandSlot2;
        public bool leftHandSlot1, leftHandSlot2;
        HandEquipmentSlotUI[] handEquipmentSlotUI;
        private void Start() {
            handEquipmentSlotUI = GetComponentsInChildren<HandEquipmentSlotUI>();
        }

        // 다크소울 처럼 오른손과 왼손에 각각 여러개의 무기들을 지니고있음
        // 장비창에 플레이어가 현재 양손에 들고있는 무기들을 표시한다.
        public void LoadWeaponOnEquipmentScreen(PlayerInventory playerInventory) {
            for (int i = 0; i < handEquipmentSlotUI.Length; i++) {
                if (handEquipmentSlotUI[i].rightHandSlot1) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlot[0]);
                }
                //else if (handEquipmentSlotUI[i].rightHandSlot2) {
                //handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlot[1]);
                //}
                else if (handEquipmentSlotUI[i].leftHandSlot1) {
                    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlot[0]);
                }
                // else {
                //    handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlot[1]);
                //}
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