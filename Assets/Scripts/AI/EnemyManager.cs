using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace sg {
    public class EnemyManager : CharacterManager {
        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyStats enemyStats;

        public bool isPerformingAction;
        public bool isInteracting;
        public State currentState;
        public CharacterStats currentTarget;
        public NavMeshAgent navmeshAgent;
        public Rigidbody enemyRigidbody;

        public float rotationSpeed = 15;
        public float maximumAttackRange = 1.5f;
        [Header("AI Settings")]
        public float detectionRadius = 20;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;

        private void Awake() {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyStats = GetComponent<EnemyStats>();
            navmeshAgent = GetComponentInChildren<NavMeshAgent>();
            enemyRigidbody = GetComponent<Rigidbody>();
            backStabCollider = GetComponentInChildren<BackStabCollider>();
            navmeshAgent.enabled = false;
        }

        private void Start() {
            enemyRigidbody.isKinematic = false;
        }

        private void Update() {
            isInteracting = enemyAnimatorManager.anim.GetBool("isInteracting");
            HandleRecoveryTimer();
            enemyAnimatorManager.anim.SetBool("isDead", enemyStats.isDead);
        }

        private void FixedUpdate() {
            HandleStateMachine();
        }

        // 타겟의 유무와 타겟과의 거리를 통해 현재 행동을 결정한다
        private void HandleStateMachine() {
            if (currentState != null) {
                State nextState = currentState.Tick(this, enemyStats, enemyAnimatorManager);
                if (nextState != null) {
                    SwitchToNextState(nextState);
                }
            }
        }

        private void SwitchToNextState(State state) {
            currentState = state;
        }

        // 공격과 공격사이의 딜레이
        private void HandleRecoveryTimer() {
            if (currentRecoveryTime > 0) {
                currentRecoveryTime -= Time.deltaTime;
            }
            if (isPerformingAction) {
                if (currentRecoveryTime <= 0) {
                    isPerformingAction = false;
                }
            }
        }
    }
}
