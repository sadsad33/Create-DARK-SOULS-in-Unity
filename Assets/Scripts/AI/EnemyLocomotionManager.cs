using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace sg {
    public class EnemyLocomotionManager : MonoBehaviour {
        EnemyManager enemyManager;
        public NavMeshAgent navmeshAgent;
        EnemyAnimatorManager enemyAnimatorManager;
        public Rigidbody enemyRigidbody;
        public float distanceFromTarget;
        public float stoppingDistance = 1f;
        public float rotationSpeed = 15;

        public LayerMask detectionLayer;
        private void Awake() {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            navmeshAgent = GetComponentInChildren<NavMeshAgent>();
            enemyRigidbody = GetComponent<Rigidbody>();
        }

        private void Start() {
            navmeshAgent.enabled = false;
            enemyRigidbody.isKinematic = false;
        }


        // 목표 위치로 이동
        public void HandleMoveToTarget() {
            if (enemyManager.isPerformingAction) return;
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

            // 특정 행동중이라면 정지
            // animator의 SetFloat을 이용해 방향과 속도만 설정
            if (enemyManager.isPerformingAction) {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                navmeshAgent.enabled = false;
            } else {
                if (distanceFromTarget > stoppingDistance) {
                    enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                } else if (distanceFromTarget <= stoppingDistance) {
                    enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                }
            }
            HandleRotateTowardsTarget();
            navmeshAgent.transform.localPosition = Vector3.zero;
            navmeshAgent.transform.localRotation = Quaternion.identity;
        }

        // 목표 방향으로 회전
        private void HandleRotateTowardsTarget() {
            
            // 특정 행동을 하고있다면 단순히 대상을 바라보도록 회전
            if (enemyManager.isPerformingAction) {
                Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero) // 타겟이 없을경우 정면을 바라봄
                    direction = transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed / Time.deltaTime);
            } else { // NavMeshAgent를 이용한 회전
                //Vector3 relativeDirection = transform.InverseTransformDirection(navmeshAgent.desiredVelocity);
                //Vector3 targetVelocity = enemyRigidbody.velocity;
                navmeshAgent.enabled = true;
                navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                //enemyRigidbody.velocity = targetVelocity;
                transform.rotation = Quaternion.Slerp(transform.rotation, navmeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
            }
        }
    }
}