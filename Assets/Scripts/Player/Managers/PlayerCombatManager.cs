using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PlayerCombatManager : MonoBehaviour {
        InputHandler inputHandler;
        CameraHandler cameraHandler;
        PlayerManager playerManager;
        public string lastAttack;
        LayerMask backStabLayer = 1 << 12;
        LayerMask riposteLayer = 1 << 13;
        public void Awake() {
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerManager = GetComponent<PlayerManager>();
            inputHandler = GetComponent<InputHandler>();
        }

        public void HandleWeaponCombo(WeaponItem weapon) {
            if (playerManager.playerStatsManager.currentStamina <= 0) return;
            if (inputHandler.comboFlag) {
                playerManager.animator.SetBool("canDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1) {
                    //Debug.Log("�޺� ���� ����");
                    playerManager.playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                } else if (lastAttack == weapon.TH_Light_Attack_1) {
                    //Debug.Log("��� �޺� ���� ����");
                    playerManager.playerAnimatorManager.PlayTargetAnimation(weapon.TH_Light_Attack_2, true);
                }
            }
        }

        public void HandleLightAttack(WeaponItem weapon) {
            if (playerManager.playerStatsManager.currentStamina <= 0 || playerManager.isInteracting || playerManager.isClimbing || playerManager.isAtBonfire) return;
            //Debug.Log("�Ѽ� ���");
            playerManager.playerWeaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag) {
                playerManager.playerAnimatorManager.PlayTargetAnimation(weapon.TH_Light_Attack_1, true);
                lastAttack = weapon.TH_Light_Attack_1;
            } else {
                playerManager.playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
        }

        public void HandleHeavyAttack(WeaponItem weapon) {
            if (playerManager.playerStatsManager.currentStamina <= 0 || playerManager.isInteracting) return;
            playerManager.playerWeaponSlotManager.attackingWeapon = weapon;
            playerManager.playerAnimatorManager.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
            lastAttack = weapon.OH_Heavy_Attack_1;
        }

        // ���� �Է�
        // �÷��̾ ����ִ� ������ ������ ���� ���� ���� �Է¿��� �ൿ�� �޶���Ѵ�.
        #region Input Actions
        public void HandleRBAction() {
            playerManager.playerAnimatorManager.EraseHandIKForWeapon();

            if (playerManager.playerInventoryManager.rightWeapon.isMeleeWeapon) {
                PerformRBMeleeAction();
            } else if (playerManager.playerInventoryManager.rightWeapon.isMagicCaster || playerManager.playerInventoryManager.rightWeapon.isFaithCaster 
                || playerManager.playerInventoryManager.rightWeapon.isPyroCaster) {
                PerformRBSpellAction(playerManager.playerInventoryManager.rightWeapon);
            }
        }

        public void HandleLBAction() {
            PerformLBBlockingAction();
        }

        public void HandleLTAction() {
            if (playerManager.playerInventoryManager.leftWeapon.isShieldWeapon) {
                PerformLTWeaponArt(inputHandler.twoHandFlag);
            } else if (playerManager.playerInventoryManager.leftWeapon.isMeleeWeapon) {
                // ���
            }
        }
        #endregion

        #region Attack Actions
        // ���� ���� ����
        private void PerformRBMeleeAction() {
            if (playerManager.canDoCombo) {
                inputHandler.comboFlag = true;
                HandleWeaponCombo(playerManager.playerInventoryManager.rightWeapon);
                inputHandler.comboFlag = false;
            } else {
                if (playerManager.isInteracting) return;
                if (playerManager.canDoCombo) return;
                playerManager.animator.SetBool("isUsingRightHand", true);
                HandleLightAttack(playerManager.playerInventoryManager.rightWeapon);
            }
        }

        // ��â ����
        private void PerformRBSpellAction(WeaponItem weapon) {
            if (playerManager.isInteracting) return;
            if (weapon.isFaithCaster) {
                if (playerManager.playerInventoryManager.currentSpell.isFaithSpell) {
                    if (playerManager.playerStatsManager.currentFocus >= playerManager.playerInventoryManager.currentSpell.focusCost)
                        playerManager.playerInventoryManager.currentSpell.AttemptToCastSpell(playerManager.playerAnimatorManager, playerManager.playerStatsManager, playerManager.playerWeaponSlotManager);
                    else playerManager.playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                } else {
                    playerManager.playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                }
            } else if (weapon.isPyroCaster) {
                if (playerManager.playerInventoryManager.currentSpell.isPyroSpell) {
                    if (playerManager.playerStatsManager.currentFocus >= playerManager.playerInventoryManager.currentSpell.focusCost)
                        playerManager.playerInventoryManager.currentSpell.AttemptToCastSpell(playerManager.playerAnimatorManager, playerManager.playerStatsManager, playerManager.playerWeaponSlotManager);
                    else playerManager.playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                } else {
                    playerManager.playerAnimatorManager.PlayTargetAnimation("Shrugging", true);
                }
            }
        }

        private void PerformLTWeaponArt(bool isTwoHanding) {
            if (playerManager.isInteracting) return;

            if (isTwoHanding) {
                // ���� ���¶�� ������ ������ ���� ���

            } else {
                // �޼� ������ ���⸦ ���
                playerManager.playerAnimatorManager.PlayTargetAnimation(playerManager.playerInventoryManager.leftWeapon.weaponArt, true);
            }
        }

        // Animation Event���� ȣ���ϱ� ���� �Լ�
        private void SuccessfullyCastSpell() {
            playerManager.playerInventoryManager.currentSpell.SuccessfullyCastSpell(playerManager.playerAnimatorManager, playerManager.playerStatsManager, cameraHandler, playerManager.playerWeaponSlotManager);
            playerManager.animator.SetBool("isFiringSpell", true);
            //Debug.Log("��ô �̺�Ʈ");
        }

        #endregion

        #region Defense Actions
        private void PerformLBBlockingAction() {
            if (playerManager.isInteracting) return;
            // �̹� �������̶�� ��ȯ
            // ����ؼ� Block Start �ִϸ��̼��� ����Ǵ°��� ����
            if (playerManager.isBlocking) return;

            playerManager.playerAnimatorManager.PlayTargetAnimation("Block Start", false, true);
            playerManager.playerEquipmentManager.OpenBlockingCollider();
            playerManager.isBlocking = true;
        }

        #endregion
        // ����, ���� �õ�
        public void AttemptBackStabOrRiposte() {
            if (playerManager.playerStatsManager.currentStamina <= 0 || playerManager.isInteracting) return;
            RaycastHit hit; // Riposte Collider �� BackStab Collider ����
            if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = playerManager.playerWeaponSlotManager.rightHandDamageCollider;
                if (enemyCharacterManager != null) { // ����, Ȥ�� ������ ������ ����� �������� ���
                    // TODO
                    // �Ǿ� �ĺ� (�Ʊ��̳� �ڽſ��Դ� �������� �ʵ���)

                    // ���� Ȥ�� ������ �Ҷ� ������� �ʵ��� Ư�� ��ǥ�� �̵���Ŵ
                    playerManager.transform.position = enemyCharacterManager.backStabCollider.criticalDamagerStandPosition.position;
                    // ȸ���� ����
                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    float criticalDamage = playerManager.playerInventoryManager.rightWeapon.criticalDamageMultiplier * rightWeapon.physicalDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    // �ִϸ��̼� ���
                    playerManager.playerAnimatorManager.PlayTargetAnimation("Back Stab", true);
                    enemyCharacterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Back Stabbed", true);
                    enemyCharacterManager.isGrabbed = true;
                }
            } else if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = playerManager.playerWeaponSlotManager.rightHandDamageCollider;
                if (enemyCharacterManager != null && enemyCharacterManager.canBeRiposted) {
                    playerManager.transform.position = enemyCharacterManager.riposteCollider.criticalDamagerStandPosition.position;

                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    float criticalDamage = playerManager.playerInventoryManager.rightWeapon.criticalDamageMultiplier * rightWeapon.physicalDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    playerManager.playerAnimatorManager.PlayTargetAnimation("Riposte", true);
                    enemyCharacterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Riposted", true);
                    enemyCharacterManager.canBeRiposted = false;
                    enemyCharacterManager.isGrabbed = true;
                }
            }
        }
    }
}