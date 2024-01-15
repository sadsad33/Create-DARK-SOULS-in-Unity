using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyWeaponSlotManager : MonoBehaviour {
        public WeaponItem rightHandWeapon, leftHandWeapon;
        public WeaponHolderSlot rightHandSlot, leftHandSlot;
        public DamageCollider leftHandDamageCollider, rightHandDamageCollider;
        EnemyStats enemyStats;
        private void Awake() {
            enemyStats = GetComponentInParent<EnemyStats>();
            LoadWeaponHolderSlots();
        }

        private void Start() {
            LoadWeaponOnBothHands();
            LoadWeaponsDamageCollider(false);
        }

        private void LoadWeaponHolderSlots() {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // �÷��̾��� �޼հ� �����տ� �ִ� WeaponHolderSlot�� ��� �����´�.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // ���� �����̶�� ���ʿ�
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // ������ �����̶�� �����ʿ�
            }
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

        #region Handle Weapon's Poise Bonus
        public void GrantWeaponAttackingPoiseBonus() { // (Ư)�������� ���� ���� ���ε� ���ʽ� �ջ�
            // ���� ��� ���⸦ ��ü�ϴ� ��Ȳ�� ������ �����Ƿ� ���ݽ� ���ε� ���ʽ��� �������� ����
            enemyStats.totalPoiseDefense = enemyStats.totalPoiseDefense + enemyStats.offensivePoiseBonus;
        }

        public void ResetWeaponAttackingPoiseBonus() { // ������ ������ ���ε� ���ʽ� �ʱ�ȭ
            enemyStats.totalPoiseDefense = enemyStats.armorPoiseBonus;
        }
        #endregion
    }
}
