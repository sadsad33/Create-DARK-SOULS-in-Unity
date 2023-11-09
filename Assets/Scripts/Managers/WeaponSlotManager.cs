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
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // �÷��̾��� �޼հ� �����տ� �ִ� WeaponHolderSlot�� ��� �����´�.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // ���� �����̶�� ���ʿ�
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // ������ �����̶�� �����ʿ�
                else if (weaponSlot.isBackSlot) {
                    backSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (isLeft) {
                leftHandSlot.currentWeapon = weaponItem; // ������¿��� ���ƿö��� ���� ���� ���� ���⸦ ����Ѵ�.
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
                    // ����� ���ʼ��� ���⸦ ������ �ű��, �޼տ� �ִ� ����� �����Ѵ�.
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

                // ������ �ϴ� ���ϴ� �������� ���Ծ���
                rightHandSlot.currentWeapon = weaponItem; // ������¿��� ���ƿö��� ���� ���� ������ ���⸦ ����Ѵ�.
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
                quickSlots.UpdateWeaponQuickSlotsUI(false, weaponItem);
            }
        }

        #region Handle Weapon's Damage Collider
        // �ִϸ��̼� ���� event�� ������ �Լ����� ����� ��
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
