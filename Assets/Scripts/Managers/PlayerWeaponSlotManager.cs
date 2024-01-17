using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerWeaponSlotManager : MonoBehaviour {
        PlayerInventoryManager playerInventoryManager;

        [Header("Attacking Weapon")]
        public WeaponItem attackingWeapon;

        [Header("Unarmed Weapon")]
        public WeaponItem unarmedWeapon;

        [Header("Weapon Slots")]
        public WeaponHolderSlot leftHandSlot;
        public WeaponHolderSlot rightHandSlot;
        public WeaponHolderSlot backSlot;

        [Header("Damage Collider")]
        public DamageCollider leftHandDamageCollider;
        public DamageCollider rightHandDamageCollider;

        Animator animator;
        QuickSlots quickSlots;
        PlayerStatsManager playerStatsManager;
        InputHandler inputHandler;
        PlayerManager playerManager;
        private void Awake() {
            playerManager = GetComponent<PlayerManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            inputHandler = GetComponent<InputHandler>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            animator = GetComponent<Animator>();
            quickSlots = FindObjectOfType<QuickSlots>();
            LoadWeaponHolderSlots();
        }

        private void LoadWeaponHolderSlots() {
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
            LoadWeaponOnSlot(playerInventoryManager.rightWeapon, false);
            LoadWeaponOnSlot(playerInventoryManager.leftWeapon, true);
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) {
            if (weaponItem != null) {
                if (isLeft) {
                    leftHandSlot.currentWeapon = weaponItem; // ������¿��� ���ƿö��� ���� ���� ���� ���⸦ ����Ѵ�.
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    animator.CrossFade(weaponItem.Left_Hand_Idle, 0.2f);
                } else {
                    if (inputHandler.twoHandFlag) {
                        // ����� ���ʼ��� ���⸦ ������ �ű��, �޼տ� �ִ� ����� �����Ѵ�.
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.th_idle, 0.2f);
                    } else {
                        animator.CrossFade("Both Arms Empty", 0.2f);
                        backSlot.UnloadWeaponAndDestroy();
                        animator.CrossFade(weaponItem.Right_Hand_Idle, 0.2f);
                    }

                    // ������ �ϴ� ���ϴ� �������� ���Ծ���
                    rightHandSlot.currentWeapon = weaponItem; // ������¿��� ���ƿö��� ���� ���� ������ ���⸦ ����Ѵ�.
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(false, weaponItem);
                }
            } else {
                weaponItem = unarmedWeapon;
                if (isLeft) {
                    animator.CrossFade("Left Arm Empty", 0.2f);
                    playerInventoryManager.leftWeapon = unarmedWeapon;
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(true, weaponItem);
                } else {
                    animator.CrossFade("Right Arm Empty", 0.2f);
                    playerInventoryManager.rightWeapon = unarmedWeapon;
                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    quickSlots.UpdateWeaponQuickSlotsUI(false, weaponItem);
                }
            }
        }


        #region Handle Weapon's Damage Collider
        // �ִϸ��̼� ���� event�� ������ �Լ����� ����� ��
        private void LoadLeftWeaponDamageCollider() {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            leftHandDamageCollider.currentWeaponDamage = playerInventoryManager.leftWeapon.baseDamage;
            // ���� ������ DamageCollider�� ���� ���� ������ ���ε� �������� ����
            leftHandDamageCollider.poiseBreak = playerInventoryManager.leftWeapon.poiseBreak;
        }

        private void LoadRightWeaponDamageCollider() {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightHandDamageCollider.currentWeaponDamage = playerInventoryManager.rightWeapon.baseDamage;
            // ������ ������ DamageCollider�� ���� ������ ������ ���ε� �������� ����
            rightHandDamageCollider.poiseBreak = playerInventoryManager.rightWeapon.poiseBreak;
        }

        public void OpenDamageCollider() {
            if (playerManager.isUsingRightHand) {
                rightHandDamageCollider.EnableDamageCollider();
            } else if (playerManager.isUsingLeftHand) {
                leftHandDamageCollider.EnableDamageCollider();
            }
        }

        public void CloseDamageCollider() {
            if (rightHandDamageCollider != null) rightHandDamageCollider.DisableDamageCollider();
            if (leftHandDamageCollider != null) leftHandDamageCollider.DisableDamageCollider();
        }

        #endregion

        #region Handle Weapon's Stamina Drainage
        public void DrainStaminaLightAttack() {
            playerStatsManager.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack() {
            playerStatsManager.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        #endregion

        #region Handle Weapon's Poise Bonus
        public void GrantWeaponAttackingPoiseBonus() { // (Ư)�������� ���� ���� ���ε� ���ʽ� �ջ�
            playerStatsManager.totalPoiseDefense = playerStatsManager.totalPoiseDefense + attackingWeapon.offensivePoiseBonus;
        }

        public void ResetWeaponAttackingPoiseBonus() { // ������ ������ ���ε� ���ʽ� �ʱ�ȭ
            playerStatsManager.totalPoiseDefense = playerStatsManager.armorPoiseBonus;
        }
        #endregion
    }
}
