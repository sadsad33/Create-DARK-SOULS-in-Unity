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
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>(); // 플레이어의 왼손과 오른손에 있는 WeaponHolderSlot을 모두 가져온다.
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
                if (weaponSlot.isLeftHandSlot)
                    leftHandSlot = weaponSlot; // 왼쪽 슬롯이라면 왼쪽에
                else if (weaponSlot.isRightHandSlot)
                    rightHandSlot = weaponSlot; // 오른쪽 슬롯이라면 오른쪽에
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
        public void GrantWeaponAttackingPoiseBonus() { // (특)대형무기 공격 도중 강인도 보너스 합산
            // 적의 경우 무기를 교체하는 상황이 흔하지 않으므로 공격시 강인도 보너스를 스탯으로 가짐
            enemyStatsManager.totalPoiseDefense = enemyStatsManager.totalPoiseDefense + enemyStatsManager.offensivePoiseBonus;
        }

        public void ResetWeaponAttackingPoiseBonus() { // 공격이 끝나면 강인도 보너스 초기화
            enemyStatsManager.totalPoiseDefense = enemyStatsManager.armorPoiseBonus;
        }
        #endregion
    }
}
