using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class WeaponSlotManager : MonoBehaviour {
        public WeaponItem attackingWeapon;
        WeaponHolderSlot leftHandSlot, rightHandSlot, backSlot;
        DamageCollider leftHandDamageCollider, rightHandDamageCollider;
        Animator animator;
        QuickSlots quickSlots;
        PlayerStats playerStats;
        InputHandler inputHandler;
        private void Awake() {
            inputHandler = GetComponentInParent<InputHandler>();
            animator = GetComponent<Animator>();
            quickSlots = FindObjectOfType<QuickSlots>();
            playerStats = GetComponentInParent<PlayerStats>();
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // 플레이어의 왼손과 오른손에 있는 WeaponHolderSlot을 모두 가져온다.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // 왼쪽 슬롯이라면 왼쪽에
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // 오른쪽 슬롯이라면 오른쪽에
                else if (weaponSlot.isBackSlot) {
                    backSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (isLeft) {
                leftHandSlot.currentWeapon = weaponItem; // 양잡상태에서 돌아올때를 위해 현재 왼쪽 무기를 기억한다.
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
                if (inputHandler.twoHandFlag) {
                    // 양잡시 왼쪽손의 무기를 등으로 옮기고, 왼손에 있는 무기는 제거한다.
                    backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                    leftHandSlot.UnloadWeaponAndDestroy();
                    animator.CrossFade(weaponItem.th_idle, 0.2f);
                } else {
                    #region Handle Right Weapon Idle Animation
                    animator.CrossFade("Both Arms Empty", 0.2f);
                    backSlot.UnloadWeaponAndDestroy();
                    if (weaponItem != null) {
                        animator.CrossFade(weaponItem.Right_Hand_Idle, 0.2f);
                    } else {
                        animator.CrossFade("Right Arm Empty", 0.2f);
                    }
                    #endregion
                }

                // 양잡을 하던 안하던 오른쪽은 변함없음
                rightHandSlot.currentWeapon = weaponItem; // 양잡상태에서 돌아올때를 위해 현재 오른쪽 무기를 기억한다.
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
                quickSlots.UpdateWeaponQuickSlotsUI(false, weaponItem);
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
