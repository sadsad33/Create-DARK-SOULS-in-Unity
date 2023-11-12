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
                    //Debug.Log("�޺� ���� ����");
                    animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                } else if (lastAttack == weapon.TH_Light_Sword_Attack_1) {
                    animatorHandler.PlayTargetAnimation(weapon.TH_Light_Sword_Attack_2, true);
                } else if (lastAttack == weapon.UnarmedAttack1) {
                    animatorHandler.PlayTargetAnimation(weapon.UnarmedAttack2, true);
                }
            }
        }

        public void HandleUnarmedAttack(WeaponItem weapon) {
            //Debug.Log("���ָ� ����");
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.UnarmedAttack1, true);
            lastAttack = weapon.UnarmedAttack1;
        }

        public void HandleLightAttack(WeaponItem weapon) {
            //Debug.Log("�Ѽ� ���");
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

        // ���� �Է�
        // �÷��̾ ����ִ� ������ ������ ���� ���� ���� �Է¿��� �ൿ�� �޶���Ѵ�.
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
        // ���� ����
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

        // ��â ����
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

        // Animation Event���� ȣ���ϱ� ���� �Լ�
        private void SuccessfullyCastSpell() {
            playerInventory.currentSpell.SuccessfullyCastSpell(animatorHandler, playerStats);
        }
        #endregion

        // ����, ���� �õ�
        public void AttemptBackStabOrRiposte() {
            RaycastHit hit;
            if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer)) {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                if (enemyCharacterManager != null) { // ����, Ȥ�� ������ ������ ����� �������� ���
                    // TODO
                    // �Ǿ� �ĺ� (�Ʊ��̳� �ڽſ��Դ� �������� �ʵ���)

                    // ���� Ȥ�� ������ �Ҷ� ������� �ʵ��� Ư�� ��ǥ�� �̵���Ŵ
                    playerManager.transform.position = enemyCharacterManager.backStabCollider.backStabberStandPoint.position;
                    // ȸ������ ����
                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;
                    
                    // �ִϸ��̼� ���
                    animatorHandler.PlayTargetAnimation("Back Stab", true);
                    enemyCharacterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Back Stabbed", true);
                    // ������ ����
                }
            }
        }
    }
}