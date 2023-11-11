using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PlayerAttacker : MonoBehaviour {
        AnimatorHandler animatorHandler;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;
        PlayerStats playerStats;
        public string lastAttack;

        public void Awake() {
            animatorHandler = GetComponent<AnimatorHandler>();
            playerStats = GetComponentInParent<PlayerStats>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            playerManager = GetComponentInParent<PlayerManager>();
            inputHandler = GetComponentInParent<InputHandler>();
            weaponSlotManager = GetComponent<WeaponSlotManager>();
        }

        public void HandleWeaponCombo(WeaponItem weapon) {
            if (inputHandler.comboFlag) {
                animatorHandler.anim.SetBool("canDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1) {
                    //Debug.Log("콤보 공격 실행");
                    animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                } else if (lastAttack == weapon.TH_Light_Sword_Attack_1) {
                    animatorHandler.PlayTargetAnimation(weapon.TH_Light_Sword_Attack_2, true);
                } else if (lastAttack == weapon.UnarmedAttack1) {
                    animatorHandler.PlayTargetAnimation(weapon.UnarmedAttack2, true);
                }
            }
        }

        public void HandleUnarmedAttack(WeaponItem weapon) {
            //Debug.Log("맨주먹 공격");
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.UnarmedAttack1, true);
            lastAttack = weapon.UnarmedAttack1;
        }

        public void HandleLightAttack(WeaponItem weapon) {
            //Debug.Log("한손 약공");
            weaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag) {
                animatorHandler.PlayTargetAnimation(weapon.TH_Light_Sword_Attack_1, true);
                lastAttack = weapon.TH_Light_Sword_Attack_1;
            } else {
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
        }

        public void HandleHeavyAttack(WeaponItem weapon) {
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
            lastAttack = weapon.OH_Heavy_Attack_1;
        }

        // 공격 입력
        // 플레이어가 들고있는 무기의 종류에 따라 같은 공격 입력에도 행동이 달라야한다.
        #region Input Actions
        public void HandleRBAction() {
            if (playerInventory.rightWeapon.isMeleeWeapon) {
                PerformRBMeleeAction();
            } else if (playerInventory.rightWeapon.isMagicCaster || playerInventory.rightWeapon.isFaithCaster || playerInventory.rightWeapon.isPyroCaster) {
                PerformRBSpellAction(playerInventory.rightWeapon);
            }
        }
        #endregion

        #region Attack Actions
        // 근접 공격
        private void PerformRBMeleeAction() {
            if (playerManager.canDoCombo) {
                inputHandler.comboFlag = true;
                HandleWeaponCombo(playerInventory.rightWeapon);
                inputHandler.comboFlag = false;
            } else {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;
                animatorHandler.anim.SetBool("isUsingRightHand", true);
                if (playerInventory.currentRightWeaponIndex != -1) {
                    HandleLightAttack(playerInventory.rightWeapon);
                } else if (playerInventory.currentRightWeaponIndex == -1) {
                    HandleUnarmedAttack(playerInventory.rightWeapon);
                }
            }
        }

        // 영창 공격
        private void PerformRBSpellAction(WeaponItem weapon) {
            if (playerManager.isInteracting) return;

            if (weapon.isFaithCaster) {
                if (playerInventory.currentSpell != null && playerInventory.currentSpell.isFaithSpell) {
                    if (playerStats.currentFocus >= playerInventory.currentSpell.focusCost)
                        playerInventory.currentSpell.AttemptToCastSpell(animatorHandler, playerStats);
                    else animatorHandler.PlayTargetAnimation("Shrugging", true);
                }
            }
        }

        // Animation Event에서 호출하기 위한 함수
        private void SuccessfullyCastSpell() {
            playerInventory.currentSpell.SuccessfullyCastSpell(animatorHandler, playerStats);
        }
        #endregion
    }
}