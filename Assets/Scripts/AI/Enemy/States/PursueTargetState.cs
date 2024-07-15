using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class PursueTargetState : State {
        public CombatStanceState combatStanceState;
        public RotateTowardsTargetState rotateTowardsTargetState;
        public DeadState deadState;
        public override State Tick(EnemyManager enemyManager, EnemyStatsManager enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            if (enemyStats.isDead) return deadState;
            // ��ǥ ����
            // ���� ��Ÿ����� Ÿ���� ������ Combat Stance State�� ��
            // Ÿ���� ���� ��Ÿ� ������ ������ Pursue Target State
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            HandleRotateTowardsTarget(enemyManager);

            // �ൿ ���̶��
            if (enemyManager.isInteracting) {
                enemyManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime); // ���ڸ��� ����
                return this;
            }

            if (distanceFromTarget <= enemyManager.maximumAggroRadius) { 
                return combatStanceState;
            } else {
                enemyManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                return this;
            }
        }

        // ��ǥ �������� ȸ��
        private void HandleRotateTowardsTarget(EnemyManager enemyManager) { // ȸ�� ����
            //Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity); // ���� ��ǥ�� -> ���� ��ǥ��
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