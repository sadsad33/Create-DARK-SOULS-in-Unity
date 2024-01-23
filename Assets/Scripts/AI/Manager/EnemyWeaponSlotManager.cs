using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyWeaponSlotManager : CharacterWeaponSlotManager {
        public WeaponItem rightHandWeapon, leftHandWeapon;
        EnemyStatsManager enemyStatsManager;
        EnemyEffectsManager enemyEffectsManager;
        private void Awake() {
            enemyStatsManager = GetComponent<EnemyStatsManager>();
            enemyEffectsManager = GetComponent<EnemyEffectsManager>();
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
                leftHandDamageCollider.physicalDamage = leftHandWeapon.physicalDamage;
                leftHandDamageCollider.fireDamage = leftHandWeapon.fireDamage;
                leftHandDamageCollider.teamIDNumber = enemyStatsManager.teamIDNumber;
                enemyEffectsManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
            } else {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
                rightHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
                rightHandDamageCollider.physicalDamage = rightHandWeapon.physicalDamage;
                rightHandDamageCollider.fireDamage = rightHandWeapon.fireDamage;
                rightHandDamageCollider.teamIDNumber = enemyStatsManager.teamIDNumber;
                enemyEffectsManager.rightWeaponFX = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
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
            enemyEffectsManager.PlayWeaponFX(false);
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void CloseDamageCollider() {
            enemyEffectsManager.StopWeaponFX(false);
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
            enemyStatsManager.totalPoiseDefense = enemyStatsManager.totalPoiseDefense + enemyStatsManager.offensivePoiseBonus;
        }

        public void ResetWeaponAttackingPoiseBonus() { // ������ ������ ���ε� ���ʽ� �ʱ�ȭ
            enemyStatsManager.totalPoiseDefense = enemyStatsManager.armorPoiseBonus;
        }
        #endregion
    }
}
