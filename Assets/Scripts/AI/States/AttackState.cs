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

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position); // 타겟과의 거리
            RotateTowardsTargetWhileAttacking(enemyManager);

            if (distanceFromTarget > enemyManager.maximumAggroRadius) { // 공격 사거리를 벗어나면 추적 상태로 전이
                return pursueTargetState;
            }

            if (willDoComboOnNextAttack && enemyManager.canDoCombo) {
                // 콤보 공격
                AttackTargetWithCombo(enemyAnimatorManager, enemyManager);
            }

            if (!hasPerformedAttack) {
                // 공격
                AttackTarget(enemyAnimatorManager, enemyManager);
                // 콤보 공격 여부 결정
                RollForComboChance(enemyManager);
            }

            if (willDoComboOnNextAttack && hasPerformedAttack) {
                return this; 
            }

            return rotateTowardsTargetState;
        }

        private void AttackTarget(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager) {
            //Debug.Log("공격");
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            Debug.Log("목표와의 사거리 : " + distanceFromTarget);
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            enemyManager.currentRecoveryTime = currentAttack.recoveryTime; // 대기시간 설정
            hasPerformedAttack = true;
        }

        private void AttackTargetWithCombo(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager) {
            //Debug.Log("콤보 공격");
            willDoComboOnNextAttack = false;
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            enemyManager.currentRecoveryTime = currentAttack.recoveryTime; // 대기시간 설정
            currentAttack = null;
        }

        private void RotateTowardsTargetWhileAttacking(EnemyManager enemyManager) {
            //if (enemyManager.isInteracting) return;

            // 공격 모션도중 회전이 가능한 구간이 있음
            // 공격 도중의 회전에만 관여하도록
            if (enemyManager.canRotate && enemyManager.isInteracting) {
                Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero) // 타겟이 없을경우 정면을 바라봄
                    direction = enemyManager.transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
        }

        private void RollForComboChance(EnemyManager enemyManager) {
            float comboChance = Random.Range(0, 100);

            if (enemyManager.allowAIToPerformCombos && comboChance <= enemyManager.comboLikelyHood) {
                if (currentAttack.comboAction != null) {
                    willDoComboOnNextAttack = true;
                    currentAttack = currentAttack.comboAction;
                } else {
                    willDoComboOnNextAttack = false;
                    currentAttack = null;
                }
            }
        }
    }
}

