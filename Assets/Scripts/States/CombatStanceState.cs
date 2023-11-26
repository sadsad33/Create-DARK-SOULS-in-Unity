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
            //Debug.Log("ȸ��");
            if (enemyManager.isInteracting) return;
            
            // Ư�� �ൿ�� �ϰ��ִٸ� �ܼ��� ����� �ٶ󺸵��� ȸ��
            if (enemyManager.isPerformingAction) {
                //Debug.Log("�Ϲ� ȸ��");
                Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero) // Ÿ���� ������� ������ �ٶ�
                    direction = enemyManager.transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            } else { // NavMeshAgent�� �̿��� ȸ��
                //Debug.Log("NavMeshAgent�� �̿��� ȸ��");
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
