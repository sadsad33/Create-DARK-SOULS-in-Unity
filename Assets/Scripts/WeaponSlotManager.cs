using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class WeaponSlotManager : MonoBehaviour {
        public WeaponItem attackingWeapon;
        WeaponHolderSlot leftHandSlot, rightHandSlot;
        DamageCollider leftHandDamageCollider, rightHandDamageCollider;
        Animator animator;
        QuickSlots quickSlots;
        PlayerStats playerStats;
        private void Awake() {
            animator = GetComponent<Animator>();
            quickSlots = FindObjectOfType<QuickSlots>();
            playerStats = GetComponentInParent<PlayerStats>();
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // 플레이어의 왼손과 오른손에 있는 WeaponHolderSlot을 모두 가져온다.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // 왼쪽 슬롯이라면 왼쪽에
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // 오른쪽 슬롯이라면 오른쪽에
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (isLeft) {
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
                quickSlots.UpdateWeaponQuickSlotsUI(true, weaponItem);

                #region Handle Left Weapon Idle Animation
                if (weaponItem != null) {
                    animator.CrossFade(weaponItem.Left_Hand_Idle, 0.2f);
                } else {
                    animator.CrossFade("Left Arm Empty", 0.2f);
                }
                #endregion
            } else {
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
                quickSlots.UpdateWeaponQuickSlotsUI(false, weaponItem);
                
                #region Handle Right Weapon Idle Animation
                if (weaponItem != null) {
                    animator.CrossFade(weaponItem.Right_Hand_Idle, 0.2f);
                } else {
                    animator.CrossFade("Right Arm Empty", 0.2f);
                }
                #endregion
            }
        }

        #region Handle Weapon's Damage Collider
        // 애니메이션 내의 event로 다음의 함수들을 사용할 것
        private void LoadLeftWeaponDamageCollider() {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        private void LoadRightWeaponDamageCollider() {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        public void OpenRightDamageCollider() {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void CloseRightDamageCollider() {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void OpenLeftDamageCollider() {
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void CloseLeftDamageCollider() {
            leftHandDamageCollider.DisableDamageCollider();
        }

        #endregion

        #region Handle Weapon's Stamina Drainage
        public void DrainStaminaLightAttack() {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack() {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion
    }
}
