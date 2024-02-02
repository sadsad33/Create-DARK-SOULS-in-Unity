using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class AttackState : State {

        public RotateTowardsTargetState rotateTowardsTargetState;
        public CombatStanceState combatStanceState;
        public PursueTargetState pursueTargetState;
        public EnemyAttackActions currentAttack;

        bool willDoComboOnNextAttack = false;
        public bool hasPerformedAttack = false;

        public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position); // Ÿ�ٰ��� �Ÿ�
            RotateTowardsTargetWhileAttacking(enemyManager);
            
            if (distanceFromTarget > enemyManager.maximumAggroRadius) { // ���� ��Ÿ��� ����� ���� ���·� ����
                return pursueTargetState;
            }

            if (willDoComboOnNextAttack && enemyManager.canDoCombo) {
                // �޺� ����
                AttackTargetWithCombo(enemyAnimatorManager, enemyManager);
            }

            //Debug.Log(hasPerformedAttack);
            if (!hasPerformedAttack) {
                // ����
                //Debug.Log("����?");
                AttackTarget(enemyAnimatorManager, enemyManager);
                // �޺� ���� ���� ����
                //RollForComboChance(enemyManager);
            }

            if (willDoComboOnNextAttack && hasPerformedAttack) {
                return this;
            }

            return rotateTowardsTargetState;
        }

        private void AttackTarget(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager) {
            //Debug.Log(currentAttack);
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //enemyAnimatorManager.PlayWeaponTrailFX();
            enemyAnimatorManager.anim.SetBool("isUsingRightHand", currentAttack.isRightHandedAction);
            enemyAnimatorManager.anim.SetBool("isUsingLeftHand", !currentAttack.isRightHandedAction);
            hasPerformedAttack = true;
            RollForComboChance(enemyManager);
            if (!willDoComboOnNextAttack) {
                //Debug.Log("�޺� ����");
                enemyManager.currentRecoveryTime = currentAttack.recoveryTime; // ���ð� ����
                currentAttack = null;
            }
        }

        private void AttackTargetWithCombo(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager) {
            willDoComboOnNextAttack = false;
            enemyAnimatorManager.anim.SetBool("isUsingRightHand", currentAttack.isRightHandedAction);
            enemyAnimatorManager.anim.SetBool("isUsingLeftHand", !currentAttack.isRightHandedAction);
            //Debug.Log(currentAttack);
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //enemyAnimatorManager.PlayWeaponTrailFX();
            enemyManager.currentRecoveryTime = currentAttack.recoveryTime; // ���ð� ����
            currentAttack = null;
        }

        private void RotateTowardsTargetWhileAttacking(EnemyManager enemyManager) {
            //if (enemyManager.isInteracting) return;

            // ���� ��ǵ��� ȸ���� ������ ������ ����
            // ���� ������ ȸ������ �����ϵ���
            if (enemyManager.canRotate && enemyManager.isInteracting) {
                Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero) // Ÿ���� ������� ������ �ٶ�
                    direction = enemyManager.transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
        }

        private void RollForComboChance(EnemyManager enemyManager) {
            float comboChance = Random.Range(0, 100);
            
            if (enemyManager.allowAIToPerformCombos && comboChance <= enemyManager.comboLikelyHood) {
                if (currentAttack.canCombo) {
                    willDoComboOnNextAttack = true;
                    currentAttack = currentAttack.comboAction;
                } else {
                    willDoComboOnNextAttack = false;
                    //currentAttack = null;
                }
            }
        }
    }
}

