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
                    //Debug.Log("�޺� ���� ����");
                    playerAnimatorManager.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                } else if (lastAttack == weapon.TH_Light_Attack_1) {
                    //Debug.Log("��� �޺� ���� ����");
                    playerAnimatorManager.PlayTargetAnimation(weapon.TH_Light_Attack_2, true);
                }
            }
        }

        public void HandleLightAttack(WeaponItem weapon) {
            if (playerStatsManager.currentStamina <= 0 || playerManager.isInteracting || playerManager.isClimbing) return;
            //Debug.Log("�Ѽ� ���");
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

        // ���� �Է�
        // �÷��̾ ����ִ� ������ ������ ���� ���� ���� �Է¿��� �ൿ�� �޶���Ѵ�.
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
                // ���
            }
        }
        #endregion

        #region Attack Actions
        // ���� ���� ����
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

        // ��â ����
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
                // ���� ���¶�� ������ ������ ���� ���

            } else {
                // �޼� ������ ���⸦ ���
                playerAnimatorManager.PlayTargetAnimation(playerInventoryManager.leftWeapon.weaponArt, true);
            }
        }

        // Animation Event���� ȣ���ϱ� ���� �Լ�
        private void SuccessfullyCastSpell() {
            playerInventoryManager.currentSpell.SuccessfullyCastSpell(playerAnimatorManager, playerStatsManager, cameraHandler, playerWeaponSlotManager);
            playerAnimatorManager.anim.SetBool("isFiringSpell", true);
            //Debug.Log("��ô �̺�Ʈ");
        }

        #endregion

        #region Defense Actions
        private void PerformLBBlockingAction() {
            if (playerManager.isInteracting) return;
            // �̹� �������̶�� ��ȯ
            // ����ؼ� Block Start �ִϸ��̼��� ����Ǵ°��� ����
            if (playerManager.isBlocking) return;

            playerAnimatorManager.PlayTargetAnimation("Block Start", false, true);
            playerEquipmentManager.OpenBlockingCollider();
            playerManager.isBlocking = true;
        }

        #endregion
        // ����, ���� �õ�
        public void AttemptBackStabOrRiposte() {
            if (playerStatsManager.currentStamina <= 0 || playerManager.isInteracting) return;
            RaycastHit hit; // Riposte Collider �� BackStab Collider ����
            if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = playerWeaponSlotManager.rightHandDamageCollider;
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

                    float criticalDamage = playerInventoryManager.rightWeapon.criticalDamageMultiplier * rightWeapon.physicalDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    // �ִϸ��̼� ���
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