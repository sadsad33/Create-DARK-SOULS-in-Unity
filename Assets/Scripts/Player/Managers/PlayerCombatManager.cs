using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerCombatManager : MonoBehaviour {
        PlayerManager player;

        public string lastAttack;
        LayerMask backStabLayer = 1 << 12;
        LayerMask riposteLayer = 1 << 13;

        public ProjectileSpell currentSpelledProjectile;
        public void Awake() {
            player = GetComponent<PlayerManager>();
        }

        public void HandleWeaponCombo(WeaponItem weapon) {
            if (player.playerStatsManager.currentStamina <= 0) return;
            if (player.inputHandler.comboFlag) {
                player.animator.SetBool("canDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1) {
                    //Debug.Log("콤보 공격 실행");
                    player.playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                } else if (lastAttack == weapon.TH_Light_Attack_1) {
                    //Debug.Log("양손 콤보 공격 실행");
                    player.playerAnimatorManager.PlayTargetAnimation(weapon.TH_Light_Attack_2, true);
                }
            }
        }

        public void HandleLightAttack(WeaponItem weapon) {
            if (player.playerStatsManager.currentStamina <= 0 || player.isInteracting || player.characterNetworkManager.isClimbing.Value || player.playerNetworkManager.isAtBonfire.Value) return;
            //Debug.Log("한손 약공");
            player.playerWeaponSlotManager.attackingWeapon = weapon;
            if (player.inputHandler.twoHandFlag) {
                player.playerAnimatorManager.PlayTargetAnimation(weapon.TH_Light_Attack_1, true);
                lastAttack = weapon.TH_Light_Attack_1;
            } else {
                player.playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
        }

        public void HandleHeavyAttack(WeaponItem weapon) {
            if (player.playerStatsManager.currentStamina <= 0 || player.isInteracting) return;
            player.playerWeaponSlotManager.attackingWeapon = weapon;
            player.playerAnimatorManager.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
            player.UpdateWhichHandCharacterIsUsing(true);
            lastAttack = weapon.OH_Heavy_Attack_1;
        }

        // 공격 입력
        // 플레이어가 들고있는 무기의 종류에 따라 같은 공격 입력에도 행동이 달라야한다.
        #region Input Actions
        public void HandleRBAction() {
            player.playerAnimatorManager.EraseHandIKForWeapon();

            if (player.playerInventoryManager.rightWeapon.isMeleeWeapon) {
                PerformRBMeleeAction();
            } else if (player.playerInventoryManager.rightWeapon.isMagicCaster || player.playerInventoryManager.rightWeapon.isFaithCaster
                || player.playerInventoryManager.rightWeapon.isPyroCaster) {
                PerformRBSpellAction(player.playerInventoryManager.rightWeapon);
            }
        }

        public void HandleLBAction() {
            PerformLBBlockingAction();
        }

        public void HandleLTAction() {
            if (player.playerInventoryManager.leftWeapon.isShieldWeapon) {
                PerformLTWeaponArt(player.inputHandler.twoHandFlag);
            } else if (player.playerInventoryManager.leftWeapon.isMeleeWeapon) {
                // 약공
            }
        }
        #endregion

        #region Attack Actions
        // 근접 공격 수행
        private void PerformRBMeleeAction() {
            if (player.canDoCombo) {
                player.inputHandler.comboFlag = true;
                HandleWeaponCombo(player.playerInventoryManager.rightWeapon);
                player.inputHandler.comboFlag = false;
            } else {
                if (player.isInteracting) return;
                if (player.canDoCombo) return;
                player.UpdateWhichHandCharacterIsUsing(true);
                HandleLightAttack(player.playerInventoryManager.rightWeapon);
            }
        }

        // 영창 공격
        private void PerformRBSpellAction(WeaponItem weapon) {
            //if (!player.IsOwner) return;
            if (player.isInteracting) return;
            player.UpdateWhichHandCharacterIsUsing(true);
            if (weapon.isFaithCaster) {
                if (player.playerInventoryManager.currentSpell.isFaithSpell) {
                    if (player.playerStatsManager.currentFocus >= player.playerInventoryManager.currentSpell.focusCost)
                        player.playerInventoryManager.currentSpell.AttemptToCastSpell(player);
                    else player.playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                } else {
                    player.playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                }
            } else if (weapon.isPyroCaster) {
                if (player.playerInventoryManager.currentSpell.isPyroSpell) {
                    if (player.playerStatsManager.currentFocus >= player.playerInventoryManager.currentSpell.focusCost)
                        player.playerInventoryManager.currentSpell.AttemptToCastSpell(player);
                    else player.playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                } else {
                    player.playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                }
            }
            player.playerCombatManager.currentSpelledProjectile = null;
        }

        private void PerformLTWeaponArt(bool isTwoHanding) {
            if (player.isInteracting) return;

            if (isTwoHanding) {
                // 양잡 상태라면 오른손 무기의 전기 사용

            } else {
                // 왼손 무기의 전기를 사용
                player.playerAnimatorManager.PlayTargetAnimation(player.playerInventoryManager.leftWeapon.weaponArt, true);
                player.UpdateWhichHandCharacterIsUsing(false);
            }
        }

        // Animation Event에서 호출하기 위한 함수
        private void SuccessfullyCastSpell() {
            if (!player.IsOwner) return;
            player.playerInventoryManager.currentSpell.SuccessfullyCastSpell(player);
            player.animator.SetBool("isFiringSpell", true);
            //Debug.Log("투척 이벤트");
        }

        #endregion

        #region Defense Actions
        private void PerformLBBlockingAction() {
            if (player.isInteracting) return;
            // 이미 막기중이라면 반환
            // 계속해서 Block Start 애니메이션이 실행되는것을 방지
            if (player.characterNetworkManager.isBlocking.Value) return;

            player.playerAnimatorManager.PlayTargetAnimation("Block Start", false, true);
            player.playerEquipmentManager.OpenBlockingCollider();
            player.characterNetworkManager.isBlocking.Value = true;
            if (player.characterNetworkManager.isTwoHandingWeapon.Value) player.UpdateWhichHandCharacterIsUsing(true);
            else player.UpdateWhichHandCharacterIsUsing(false);
        }

        #endregion
        // 뒤잡, 앞잡 시도
        public void AttemptBackStabOrRiposte() {
            if (player.playerStatsManager.currentStamina <= 0 || player.isInteracting) return;
            RaycastHit hit; // Riposte Collider 와 BackStab Collider 감지
            if (Physics.Raycast(player.inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = player.playerWeaponSlotManager.rightHandDamageCollider;
                if (enemyCharacterManager != null) { // 뒤잡, 혹은 앞잡이 가능한 대상을 포착했을 경우
                    // TODO
                    // 피아 식별 (아군이나 자신에게는 가능하지 않도록)

                    // 뒤잡 혹은 앞잡을 할때 어색하지 않도록 특정 좌표로 이동시킴
                    player.transform.position = enemyCharacterManager.backStabCollider.criticalDamagerStandPosition.position;
                    // 회전값 조정
                    Vector3 rotationDirection = player.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - player.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, 500 * Time.deltaTime);
                    player.transform.rotation = targetRotation;

                    float criticalDamage = player.playerInventoryManager.rightWeapon.criticalDamageMultiplier * rightWeapon.physicalDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    // 애니메이션 재생
                    player.playerAnimatorManager.PlayTargetAnimation("Back Stab", true);
                    player.UpdateWhichHandCharacterIsUsing(true);
                    enemyCharacterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Back Stabbed", true);
                    enemyCharacterManager.isGrabbed = true;
                }
            } else if (Physics.Raycast(player.inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = player.playerWeaponSlotManager.rightHandDamageCollider;
                if (enemyCharacterManager != null && enemyCharacterManager.canBeRiposted) {
                    player.transform.position = enemyCharacterManager.riposteCollider.criticalDamagerStandPosition.position;

                    Vector3 rotationDirection = player.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - player.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, 500 * Time.deltaTime);
                    player.transform.rotation = targetRotation;

                    float criticalDamage = player.playerInventoryManager.rightWeapon.criticalDamageMultiplier * rightWeapon.physicalDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    player.playerAnimatorManager.PlayTargetAnimation("Riposte", true);
                    player.UpdateWhichHandCharacterIsUsing(true);
                    enemyCharacterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Riposted", true);
                    enemyCharacterManager.canBeRiposted = false;
                    enemyCharacterManager.isGrabbed = true;
                }
            }
        }
    }
}