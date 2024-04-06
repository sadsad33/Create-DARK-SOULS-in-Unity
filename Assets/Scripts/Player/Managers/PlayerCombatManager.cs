using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerCombatManager : MonoBehaviour {
        InputHandler inputHandler;
        CameraHandler cameraHandler;
        PlayerManager playerManager;
        PlayerAnimatorManager playerAnimatorManager;
        PlayerEquipmentManager playerEquipmentManager;
        PlayerInventoryManager playerInventoryManager;
        PlayerWeaponSlotManager playerWeaponSlotManager;
        PlayerStatsManager playerStatsManager;
        //PlayerEffectsManager playerEffectsManager;
        public string lastAttack;
        LayerMask backStabLayer = 1 << 12;
        LayerMask riposteLayer = 1 << 13;
        public void Awake() {
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerManager = GetComponent<PlayerManager>();
            inputHandler = GetComponent<InputHandler>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
            //playerEffectsManager = GetComponent<PlayerEffectsManager>();
        }

        public void HandleWeaponCombo(WeaponItem weapon) {
            if (playerStatsManager.currentStamina <= 0) return;
            if (inputHandler.comboFlag) {
                playerAnimatorManager.anim.SetBool("canDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1) {
                    //Debug.Log("콤보 공격 실행");
                    playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                } else if (lastAttack == weapon.TH_Light_Attack_1) {
                    //Debug.Log("양손 콤보 공격 실행");
                    playerAnimatorManager.PlayTargetAnimation(weapon.TH_Light_Attack_2, true);
                }
            }
        }

        public void HandleLightAttack(WeaponItem weapon) {
            if (playerStatsManager.currentStamina <= 0 || playerManager.isInteracting || playerManager.isClimbing) return;
            //Debug.Log("한손 약공");
            playerWeaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag) {
                playerAnimatorManager.PlayTargetAnimation(weapon.TH_Light_Attack_1, true);
                lastAttack = weapon.TH_Light_Attack_1;
            } else {
                playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
        }

        public void HandleHeavyAttack(WeaponItem weapon) {
            if (playerStatsManager.currentStamina <= 0 || playerManager.isInteracting) return;
            playerWeaponSlotManager.attackingWeapon = weapon;
            playerAnimatorManager.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
            lastAttack = weapon.OH_Heavy_Attack_1;
        }

        // 공격 입력
        // 플레이어가 들고있는 무기의 종류에 따라 같은 공격 입력에도 행동이 달라야한다.
        #region Input Actions
        public void HandleRBAction() {
            playerAnimatorManager.EraseHandIKForWeapon();
            
            if (playerInventoryManager.rightWeapon.isMeleeWeapon) {
                PerformRBMeleeAction();
            } else if (playerInventoryManager.rightWeapon.isMagicCaster || playerInventoryManager.rightWeapon.isFaithCaster || playerInventoryManager.rightWeapon.isPyroCaster) {
                PerformRBSpellAction(playerInventoryManager.rightWeapon);
            }
        }

        public void HandleLBAction() {
            PerformLBBlockingAction();
        }

        public void HandleLTAction() {
            if (playerInventoryManager.leftWeapon.isShieldWeapon) {
                PerformLTWeaponArt(inputHandler.twoHandFlag);
            } else if (playerInventoryManager.leftWeapon.isMeleeWeapon) {
                // 약공
            }
        }
        #endregion

        #region Attack Actions
        // 근접 공격 수행
        private void PerformRBMeleeAction() {
            if (playerManager.canDoCombo) {
                inputHandler.comboFlag = true;
                HandleWeaponCombo(playerInventoryManager.rightWeapon);
                inputHandler.comboFlag = false;
            } else {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;
                playerAnimatorManager.anim.SetBool("isUsingRightHand", true);
                HandleLightAttack(playerInventoryManager.rightWeapon);
            }
        }

        // 영창 공격
        private void PerformRBSpellAction(WeaponItem weapon) {
            if (playerManager.isInteracting) return;
            if (weapon.isFaithCaster) {
                if (playerInventoryManager.currentSpell != null && playerInventoryManager.currentSpell.isFaithSpell) {
                    if (playerStatsManager.currentFocus >= playerInventoryManager.currentSpell.focusCost)
                        playerInventoryManager.currentSpell.AttemptToCastSpell(playerAnimatorManager, playerStatsManager, playerWeaponSlotManager);
                    else playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                }
            } else if (weapon.isPyroCaster) {
                if (playerInventoryManager.currentSpell != null && playerInventoryManager.currentSpell.isPyroSpell) {
                    if (playerStatsManager.currentFocus >= playerInventoryManager.currentSpell.focusCost)
                        playerInventoryManager.currentSpell.AttemptToCastSpell(playerAnimatorManager, playerStatsManager, playerWeaponSlotManager);
                    else playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                }
            }
        }

        private void PerformLTWeaponArt(bool isTwoHanding) {
            if (playerManager.isInteracting) return;

            if (isTwoHanding) {
                // 양잡 상태라면 오른손 무기의 전기 사용

            } else {
                // 왼손 무기의 전기를 사용
                playerAnimatorManager.PlayTargetAnimation(playerInventoryManager.leftWeapon.weaponArt, true);
            }
        }

        // Animation Event에서 호출하기 위한 함수
        private void SuccessfullyCastSpell() {
            playerInventoryManager.currentSpell.SuccessfullyCastSpell(playerAnimatorManager, playerStatsManager, cameraHandler, playerWeaponSlotManager);
            playerAnimatorManager.anim.SetBool("isFiringSpell", true);
            //Debug.Log("투척 이벤트");
        }

        #endregion

        #region Defense Actions
        private void PerformLBBlockingAction() {
            if (playerManager.isInteracting) return;
            // 이미 막기중이라면 반환
            // 계속해서 Block Start 애니메이션이 실행되는것을 방지
            if (playerManager.isBlocking) return;

            playerAnimatorManager.PlayTargetAnimation("Block Start", false, true);
            playerEquipmentManager.OpenBlockingCollider();
            playerManager.isBlocking = true;
        }

        #endregion
        // 뒤잡, 앞잡 시도
        public void AttemptBackStabOrRiposte() {
            if (playerStatsManager.currentStamina <= 0 || playerManager.isInteracting) return;
            RaycastHit hit; // Riposte Collider 와 BackStab Collider 감지
            if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = playerWeaponSlotManager.rightHandDamageCollider;
                if (enemyCharacterManager != null) { // 뒤잡, 혹은 앞잡이 가능한 대상을 포착했을 경우
                    // TODO
                    // 피아 식별 (아군이나 자신에게는 가능하지 않도록)

                    // 뒤잡 혹은 앞잡을 할때 어색하지 않도록 특정 좌표로 이동시킴
                    playerManager.transform.position = enemyCharacterManager.backStabCollider.criticalDamagerStandPosition.position;
                    // 회전값 조정
                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    float criticalDamage = playerInventoryManager.rightWeapon.criticalDamageMultiplier * rightWeapon.physicalDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    // 애니메이션 재생
                    playerAnimatorManager.PlayTargetAnimation("Back Stab", true);
                    enemyCharacterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Back Stabbed", true);
                    enemyCharacterManager.isGrabbed = true;
                }
            } else if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = playerWeaponSlotManager.rightHandDamageCollider;
                if (enemyCharacterManager != null && enemyCharacterManager.canBeRiposted) {
                    playerManager.transform.position = enemyCharacterManager.riposteCollider.criticalDamagerStandPosition.position;

                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    float criticalDamage = playerInventoryManager.rightWeapon.criticalDamageMultiplier * rightWeapon.physicalDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    playerAnimatorManager.PlayTargetAnimation("Riposte", true);
                    enemyCharacterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Riposted", true);
                    enemyCharacterManager.canBeRiposted = false;
                    enemyCharacterManager.isGrabbed = true;
                }
            }
        }
    }
}