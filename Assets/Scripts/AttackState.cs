using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class AttackState : State {
        public CombatStanceState combatStanceState;
        public EnemyAttackActions[] enemyAttacks;
        public EnemyAttackActions currentAttack;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // attack score 변수에 따라 수행가능한 공격들중 하나를 선택한다
            // 선택된 공격이 각도나 거리에 의해 불가능해 진다면 새로운 공격을 선택
            // 공격이 가능하다면 이동을 멈춘후 공격
            // 공격 딜레이 시간을 세팅한다.
            // Combat Stance State로 돌아감

            //Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            
            // 대상 공격
            if (enemyManager.isPerformingAction) return combatStanceState;

            if (currentAttack != null) {
                // 만약 타겟이 공격하기에 너무 가까이 있다면 새로운 공격을 선택한다.
                if (enemyManager.distanceFromTarget < currentAttack.minimumDistanceNeededToAttack) {
                    return this;
                } else if (enemyManager.distanceFromTarget < currentAttack.maximumDistanceNeededToAttack) {
                    if (enemyManager.viewableAngle <= currentAttack.maximumAttackAngle && enemyManager.viewableAngle >= currentAttack.minimumAttackAngle) {
                        if (enemyManager.currentRecoveryTime <= 0 && !enemyManager.isPerformingAction) {
                            enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                            enemyAnimatorManager.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                            enemyManager.isPerformingAction = true;
                            enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                            currentAttack = null;
                            return combatStanceState;
                        }
                    }
                }
            } else {
                GetNewAttack(enemyManager);
            }

            return combatStanceState;
        }

        // 공격 선택
        private void GetNewAttack(EnemyManager enemyManager) {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            enemyManager.distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

            int maxScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];

                if (enemyManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    enemyManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // 현재 목표가 공격 사거리 내에 있고
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // 공격 가능한 시야각내에 있다면
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];

                if (enemyManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    enemyManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) {
                        if (currentAttack != null) return; // 이미 공격중이라면
                        temporaryScore += enemyAttackAction.attackScore;

                        if (temporaryScore > randomValue) {
                            currentAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }
    }
}

