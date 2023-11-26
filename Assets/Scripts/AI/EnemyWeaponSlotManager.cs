using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyWeaponSlotManager : MonoBehaviour {
        public WeaponItem rightHandWeapon, leftHandWeapon;
        public WeaponHolderSlot rightHandSlot, leftHandSlot;
        public DamageCollider leftHandDamageCollider, rightHandDamageCollider;

        private void Awake() {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // 플레이어의 왼손과 오른손에 있는 WeaponHolderSlot을 모두 가져온다.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // 왼쪽 슬롯이라면 왼쪽에
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // 오른쪽 슬롯이라면 오른쪽에
            }
        }

        private void Start() {
            LoadWeaponOnBothHands();
            LoadWeaponsDamageCollider(false);
        }

        public void LoadWeaponOnSlot(WeaponItem weapon, bool isLeft) {
            if (isLeft) {
                leftHandSlot.currentWeapon = weapon;
                leftHandSlot.LoadWeaponModel(weapon);
            } else {
                rightHandSlot.currentWeapon = weapon;
                rightHandSlot.LoadWeaponModel(weapon);
            }
        }

        public void LoadWeaponsDamageCollider(bool isLeft) {
            if (isLeft) {
                leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
                leftHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
            } else {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
                rightHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
            }
        }
        
        public void LoadWeaponOnBothHands() {
            if (rightHandWeapon != null) {
                LoadWeaponOnSlot(rightHandWeapon, false);
            }
            if (leftHandWeapon != null) {
                LoadWeaponOnSlot(leftHandWeapon, true);
            }
        }

        public void OpenDamageCollider() {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void CloseDamageCollider() {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void DrainStaminaLightAttack() {
        }

        public void DrainStaminaHeavyAttack() {
        }

        public void EnableCombo() {
        }
        public void DisableCombo() {
        }
    }
}
