using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class AttackState : State {
        public CombatStanceState combatStanceState;
        public EnemyAttackActions[] enemyAttacks;
        public EnemyAttackActions currentAttack;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // attack score ������ ���� ���డ���� ���ݵ��� �ϳ��� �����Ѵ�
            // ���õ� ������ ������ �Ÿ��� ���� �Ұ����� ���ٸ� ���ο� ������ ����
            // ������ �����ϴٸ� �̵��� ������ ����
            // ���� ������ �ð��� �����Ѵ�.
            // Combat Stance State�� ���ư�

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);
            // ��� ����
            if (enemyManager.isPerformingAction) return combatStanceState;

            if (currentAttack != null) {
                // ���� Ÿ���� �����ϱ⿡ �ʹ� ������ �ִٸ� ���ο� ������ �����Ѵ�.
                if (distanceFromTarget < currentAttack.minimumDistanceNeededToAttack) {
                    return this;
                } else if (distanceFromTarget < currentAttack.maximumDistanceNeededToAttack) {
                    if (viewableAngle <= currentAttack.maximumAttackAngle && viewableAngle >= currentAttack.minimumAttackAngle) {
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

        // ���� ����
        private void GetNewAttack(EnemyManager enemyManager) {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            int maxScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // ���� ��ǥ�� ���� ��Ÿ� ���� �ְ�
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // ���� ������ �þ߰����� �ִٸ�
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;
            for (int i = 0; i < enemyAttacks.Length; i++) {
                EnemyAttackActions enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) {
                        if (currentAttack != null) return; // �̹� �������̶��
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
