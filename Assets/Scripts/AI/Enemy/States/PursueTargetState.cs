using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PursueTargetState : State {
        public CombatStanceState combatStanceState;
        public RotateTowardsTargetState rotateTowardsTargetState;
        public DeadState deadState;
        public override State Tick(AICharacterManager aiManager) {
            if (aiManager.aiStatsManager.isDead) return deadState;
            // 목표 추적
            // 공격 사거리내에 타겟이 들어오면 Combat Stance State가 됨
            // 타겟이 공격 사거리 밖으로 나가면 Pursue Target State
            Vector3 targetDirection = aiManager.currentTarget.transform.position - aiManager.transform.position;
            float distanceFromTarget = Vector3.Distance(aiManager.currentTarget.transform.position, aiManager.transform.position);
            HandleRotateTowardsTarget(aiManager);

            // 행동 중이라면
            if (aiManager.isInteracting) {
                aiManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime); // 제자리에 정지
                return this;
            }

            if (distanceFromTarget <= aiManager.maximumAggroRadius) {
                return combatStanceState;
            } else {
                aiManager.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                return this;
            }
        }

        // 목표 방향으로 회전
        private void HandleRotateTowardsTarget(AICharacterManager enemyManager) { // 회전 제어
            //Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity); // 월드 좌표계 -> 로컬 좌표계
            //Vector3 targetVelocity = enemyManager.enemyRigidbody.velocity;
            //enemyManager.navMeshAgent.enabled = true;
            //enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            //enemyManager.enemyRigidbody.velocity = targetVelocity;
            //enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(enemyManager.transform.rotation, tr, enemyManager.rotationSpeed * Time.deltaTime);
            enemyManager.transform.rotation = targetRotation;
        }
    }
}