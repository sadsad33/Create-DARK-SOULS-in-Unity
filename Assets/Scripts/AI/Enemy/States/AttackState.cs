using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class AttackState : State {

        public RotateTowardsTargetState rotateTowardsTargetState;
        public CombatStanceState combatStanceState;
        public PursueTargetState pursueTargetState;
        public EnemyAttackActions currentAttack;
        public DeadState deadState;

        bool willDoComboOnNextAttack = false;
        public bool hasPerformedAttack = false; // 공격 수행 여부

        public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            if (enemyStats.isDead) return deadState;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position); // 타겟과의 거리
            RotateTowardsTargetWhileAttacking(enemyManager); // 공격 도중 일부 구간에서 회전 가능
            
            if (distanceFromTarget > enemyManager.maximumAggroRadius) { // 공격 사거리를 벗어나면 추적 상태로 전이
                return pursueTargetState;
            }

            if (willDoComboOnNextAttack && enemyManager.canDoCombo) {
                // 콤보 공격
                AttackTargetWithCombo(enemyAnimatorManager, enemyManager);
            }

            //Debug.Log(hasPerformedAttack);
            if (!hasPerformedAttack) {
                //Debug.Log("공격?");
                AttackTarget(enemyAnimatorManager, enemyManager);
            }

            // 콤보 공격 플래그가 공격 이후 결정되기 때문에, 공격이 끝난후 콤보 공격 플래그가 true라면 다시 현재 상태를 반환하여 콤보 공격을 수행할수 있도록
            if (willDoComboOnNextAttack && hasPerformedAttack) {
                return this;
            }

            return rotateTowardsTargetState;
        }

        // 공격 수행
        private void AttackTarget(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager) {
            //Debug.Log(currentAttack);
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //enemyAnimatorManager.PlayWeaponTrailFX();
            enemyManager.animator.SetBool("isUsingRightHand", currentAttack.isRightHandedAction);
            enemyManager.animator.SetBool("isUsingLeftHand", !currentAttack.isRightHandedAction);
            hasPerformedAttack = true;
            RollForComboChance(enemyManager);
            if (!willDoComboOnNextAttack) {
                //Debug.Log("콤보 안함");
                enemyManager.currentRecoveryTime = currentAttack.recoveryTime; // 대기시간 설정
                currentAttack = null;
            }
        }

        private void AttackTargetWithCombo(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager) {
            willDoComboOnNextAttack = false;
            enemyManager.animator.SetBool("isUsingRightHand", currentAttack.isRightHandedAction);
            enemyManager.animator.SetBool("isUsingLeftHand", !currentAttack.isRightHandedAction);
            //Debug.Log(currentAttack);
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //enemyAnimatorManager.PlayWeaponTrailFX();
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


        // 콤보 공격을 위한 시행
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

