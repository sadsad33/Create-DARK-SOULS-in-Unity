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
        public CharacterStats currentTarget;
        public LayerMask detectionLayer;
        public float distanceFromTarget;
        public float stoppingDistance = 1f;
        public float rotationSpeed = 15;
        
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
        // 주변 오브젝트들 감지
        public void HandleDetection() {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);
            for (int i = 0; i < colliders.Length; i++) {
                // 감지한 주변 collider로부터 CharacterStats을 가져온다.
                CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();
                
                // 해당 오브젝트에 CharacterStats이 존재한다면
                if (characterStats != null) {
                    Vector3 targetDirection = characterStats.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                    // 정면과 목표물 사이의 각도가 최소 시야각과 최대 시야각 내의 범위에 있다면
                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle) {
                        currentTarget = characterStats; // 타겟을 설정한다.
                    }
                }
            }
        }

        // 목표 위치로 이동
        public void HandleMoveToTarget() {
            Vector3 targetDirection = currentTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);

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
                Vector3 direction = currentTarget.transform.position - transform.position;
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
                navmeshAgent.SetDestination(currentTarget.transform.position);
                //enemyRigidbody.velocity = targetVelocity;
                transform.rotation = Quaternion.Slerp(transform.rotation, navmeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
            }
        }
    }
}