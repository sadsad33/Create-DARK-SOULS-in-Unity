using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class BossCombatStanceState : CombatStanceState {
        [Header("Second Phase Attacks")]
        public bool hasPhaseShifted; // ������ ��ȯ ���θ� ������ ����
        public EnemyAttackActions[] secondPhaseEnemyAttacks; // 2������ ���Խ� ���� ���� ���

        protected override void GetNewAttack(AICharacterManager enemyManager) {
            if (hasPhaseShifted) {

                Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
                float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

                int maxScore = 0;
                for (int i = 0; i < secondPhaseEnemyAttacks.Length; i++) {
                    EnemyAttackActions enemyAttackAction = secondPhaseEnemyAttacks[i];

                    if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                        distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // ���� ��ǥ�� ���� ��Ÿ� ���� �ְ�
                        if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // ���� ������ �þ߰����� �ִٸ�
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
                            if (attackState.currentAttack != null) return; // �̹� �������̶��
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