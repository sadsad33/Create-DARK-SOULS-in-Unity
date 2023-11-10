using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CombatStanceState : State {
        public AttackState attackState;
        public PursueTargetState pursueTargetState;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // 공격 사거리 확인
            // 공격 대상 주위에서 걷거나 빙글빙글 돈다
            // 공격 사거리 내에 들어가면 Attack State가 된다.
            // 공격후 딜레이 상태라면 Combat Stance State로 돌아오고 타겟 주위를 배회
            // 만약 타겟이 공격 사거리 밖으로 도망가버리면 Pursue Target State가 됨.

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
