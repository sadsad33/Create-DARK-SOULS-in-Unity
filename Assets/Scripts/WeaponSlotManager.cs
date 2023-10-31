using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class WeaponSlotManager : MonoBehaviour {
        WeaponHolderSlot leftHandSlot, rightHandSlot;

        private void Awake() {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // 플레이어의 왼손과 오른손에 있는 WeaponHolderSlot을 모두 가져온다.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // 왼쪽 슬롯이라면 왼쪽에
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // 오른쪽 슬롯이라면 오른쪽에
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (isLeft)
                leftHandSlot.LoadWeaponModel(weaponItem);
            else
                rightHandSlot.LoadWeaponModel(weaponItem);
        }
    }
}
