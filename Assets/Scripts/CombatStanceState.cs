using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CombatStanceState : State {
        public AttackState attackState;
        public PursueTargetState pursueTargetState;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // ���� ��Ÿ� Ȯ��
            // ���� ��� �������� �Ȱų� ���ۺ��� ����
            // ���� ��Ÿ� ���� ���� Attack State�� �ȴ�.
            // ������ ������ ���¶�� Combat Stance State�� ���ƿ��� Ÿ�� ������ ��ȸ
            // ���� Ÿ���� ���� ��Ÿ� ������ ������������ Pursue Target State�� ��.

            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            if (enemyManager.isPerformingAction) {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }
            if (enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maximumAttackRange) {
                return attackState;
            } else if (distanceFromTarget > enemyManager.maximumAttackRange) {
                return pursueTargetState;
            } else
                return this;
        }
    }
}