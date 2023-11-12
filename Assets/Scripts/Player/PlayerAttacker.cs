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
        LayerMask backStabLayer = 1 << 12;
        
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

        // 뒤잡, 앞잡 시도
        public void AttemptBackStabOrRiposte() {
            RaycastHit hit;
            if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                if (enemyCharacterManager != null) { // 뒤잡, 혹은 앞잡이 가능한 대상을 포착했을 경우
                    // TODO
                    // 피아 식별 (아군이나 자신에게는 가능하지 않도록)

                    // 뒤잡 혹은 앞잡을 할때 어색하지 않도록 특정 좌표로 이동시킴
                    playerManager.transform.position = enemyCharacterManager.backStabCollider.backStabberStandPoint.position;
                    // 회전값도 조정
                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;
                    
                    // 애니메이션 재생
                    animatorHandler.PlayTargetAnimation("Back Stab", true);
                    enemyCharacterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Back Stabbed", true);
                    // 데미지 전달
                }
            }
        }
    }
}