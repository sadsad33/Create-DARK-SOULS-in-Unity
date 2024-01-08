using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class WeaponSlotManager : MonoBehaviour {
        public WeaponItem attackingWeapon;
        PlayerInventory playerInventory;
        [SerializeField] 
        public WeaponHolderSlot leftHandSlot, rightHandSlot, backSlot;
        [SerializeField]
        public DamageCollider leftHandDamageCollider; 
        [SerializeField]
        public DamageCollider rightHandDamageCollider;
        
        Animator animator;
        QuickSlots quickSlots;
        PlayerStats playerStats;
        InputHandler inputHandler;
        PlayerManager playerManager;
        private void Awake() {
            playerManager = GetComponentInParent<PlayerManager>();
            playerInventory = GetComponentInParent<PlayerInventory>();
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

        // �տ� ���Ⱑ �ƴ� ���𰡸� ��� �ϴ� ������ ������ ���⸦ �ٽ� �ε��ϱ� ���� �Լ�
        // �ִϸ��̼� �̺�Ʈ�� �߰��ϴ°� ���� ���� ������
        public void LoadBothWeaponsOnSlots() {
            LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            LoadWeaponOnSlot(playerInventory.leftWeapon, true);
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
            leftHandDamageCollider.currentWeaponDamage = playerInventory.leftWeapon.baseDamage;
            // ���� ������ DamageCollider�� ���� ���� ������ ���ε� �������� ����
            leftHandDamageCollider.poiseBreak = playerInventory.leftWeapon.poiseBreak;
        }

        private void LoadRightWeaponDamageCollider() {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightHandDamageCollider.currentWeaponDamage = playerInventory.rightWeapon.baseDamage;
            // ������ ������ DamageCollider�� ���� ������ ������ ���ε� �������� ����
            rightHandDamageCollider.poiseBreak = playerInventory.rightWeapon.poiseBreak;
        }

        public void OpenDamageCollider() {
            if (playerManager.isUsingRightHand) {
                rightHandDamageCollider.EnableDamageCollider();
            } else if (playerManager.isUsingLeftHand) {
                leftHandDamageCollider.EnableDamageCollider();
            }
        }

        public void CloseDamageCollider() {
            rightHandDamageCollider.DisableDamageCollider();
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
