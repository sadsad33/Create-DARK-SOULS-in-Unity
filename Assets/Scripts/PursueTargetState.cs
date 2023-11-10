using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class PursueTargetState : State {
        public CombatStanceState combatStanceState;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) {
            // ��ǥ ����
            // ���� ��Ÿ����� Ÿ���� ������ Combat Stance State�� ��
            // Ÿ���� ���� ��Ÿ� ������ ������ Pursue Target State�� ������ä ����
            if (enemyManager.isPerformingAction) { // �ൿ���̶��
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime); // ����
                return this;
            }

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

            if (distanceFromTarget > enemyManager.maximumAttackRange) {
                enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }

            HandleRotateTowardsTarget(enemyManager);
            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;

            if (distanceFromTarget <= enemyManager.maximumAttackRange) {
                return combatStanceState;
            } else {
                return this;
            }
        }

        // ��ǥ �������� ȸ��
        private void HandleRotateTowardsTarget(EnemyManager enemyManager) {
            //Debug.Log("ȸ��");

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