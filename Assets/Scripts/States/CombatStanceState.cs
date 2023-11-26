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
            HandleRotateTowardsTarget(enemyManager);

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
        private void HandleRotateTowardsTarget(EnemyManager enemyManager) {
            //Debug.Log("회전");
            if (enemyManager.isInteracting) return;
            
            // 특정 행동을 하고있다면 단순히 대상을 바라보도록 회전
            if (enemyManager.isPerformingAction) {
                //Debug.Log("일반 회전");
                Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero) // 타겟이 없을경우 정면을 바라봄
                    direction = enemyManager.transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            } else { // NavMeshAgent를 이용한 회전
                //Debug.Log("NavMeshAgent를 이용한 회전");
                //Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemyManager.enemyRigidbody.velocity;
                enemyManager.navmeshAgent.enabled = true;
                enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                enemyManager.enemyRigidbody.velocity = targetVelocity;
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
        }
    }
}
