using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BossCombatStanceState : CombatStanceState {
        [Header("Second Phase Attacks")]
        public bool hasPhaseShifted; // 페이즈 전환 여부를 저장할 변수
        public EnemyAttackActions[] secondPhaseEnemyAttacks; // 2페이즈 진입시 사용될 공격 목록

        protected override void GetNewAttack(AICharacterManager enemyManager) {
            if (hasPhaseShifted) {

                Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
                float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

                int maxScore = 0;
                for (int i = 0; i < secondPhaseEnemyAttacks.Length; i++) {
                    EnemyAttackActions enemyAttackAction = secondPhaseEnemyAttacks[i];

                    if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                        distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // 현재 목표가 공격 사거리 내에 있고
                        if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // 공격 가능한 시야각내에 있다면
                            maxScore += enemyAttackAction.attackScore;
                        }
                    }
                }

                int randomValue = Random.Range(0, maxScore);
                int temporaryScore = 0;
                for (int i = 0; i < secondPhaseEnemyAttacks.Length; i++) {
                    EnemyAttackActions enemyAttackAction = secondPhaseEnemyAttacks[i];

                    if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                        distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) {
                        if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) {
                            if (attackState.currentAttack != null) return; // 이미 공격중이라면
                            temporaryScore += enemyAttackAction.attackScore;

                            if (temporaryScore > randomValue) {
                                attackState.currentAttack = enemyAttackAction;
                                //Debug.Log(attackState.hasPerformedAttack);
                                //Debug.Log(attackState.currentAttack);
                            }
                        }
                    }
                }
            } else {
                base.GetNewAttack(enemyManager);
            }
        }
    }
}