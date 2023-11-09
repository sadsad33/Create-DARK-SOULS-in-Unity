using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PursueTargetState : State {
        public CombatStanceState combatStanceState;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // 목표 추적
            // 공격 사거리내에 타겟이 들어오면 Combat Stance State가 됨
            // 타겟이 공격 사거리 밖으로 나가면 Pursue Target State를 유지한채 추적

            if (enemyManager.isPerformingAction) return this;
            //Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            enemyManager.distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);
            //float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

            if (enemyManager.distanceFromTarget > enemyManager.maximumAttackRange) {
                enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }

            HandleRotateTowardsTarget(enemyManager);
            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;

            if (enemyManager.distanceFromTarget <= enemyManager.maximumAttackRange) {
                return combatStanceState;
            } else {
                return this;
            }
        }

        // 목표 방향으로 회전
        private void HandleRotateTowardsTarget(EnemyManager enemyManager) {
            //Debug.Log("회전");

            // 특정 행동을 하고있다면 단순히 대상을 바라보도록 회전
            if (enemyManager.isPerformingAction) {
                //Debug.Log("일반 회전");
                Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero) // 타겟이 없을경우 정면을 바라봄
                    direction = transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            } else { // NavMeshAgent를 이용한 회전
                //Debug.Log("NavMeshAgent를 이용한 회전");
                //Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemyManager.enemyRigidbody.velocity;
                enemyManager.navmeshAgent.enabled = true;
                enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                enemyManager.enemyRigidbody.velocity = targetVelocity;
                enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
        }
    }
}