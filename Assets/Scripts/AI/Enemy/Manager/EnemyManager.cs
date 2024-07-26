using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace SoulsLike {
    public class EnemyManager : CharacterManager {
        
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyStatsManager enemyStatsManager;
        EnemyEffectsManager enemyEffectsManager;
        //public bool isPerformingAction;
        public State currentState;
        public CharacterStatsManager currentTarget;
        public NavMeshAgent navMeshAgent;
        //public Rigidbody enemyRigidbody;

        public float rotationSpeed = 15;
        public float maximumAggroRadius = 1.5f; // 공격 가능 사거리

        [Header("AI Settings")]
        public float detectionRadius = 20;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;

        [Header("AI CombatSettings")]
        public bool allowAIToPerformCombos;
        public bool isPhaseShifting; // 페이즈 전환을 수행했는지 여부
        public float comboLikelyHood;

        protected override void Awake() {
            base.Awake();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            enemyStatsManager = GetComponent<EnemyStatsManager>();
            enemyEffectsManager = GetComponent<EnemyEffectsManager>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            navMeshAgent.enabled = false;
            navMeshAgent.updateRotation = false;
        }

        protected override void Start() {
            base.Start();
        }

        protected override void Update() {
            HandleRecoveryTimer();
            HandleStateMachine();

            isUsingLeftHand = animator.GetBool("isUsingLeftHand");
            isUsingRightHand = animator.GetBool("isUsingRightHand");
            isPhaseShifting = animator.GetBool("isPhaseShifting");
            isRotatingWithRootMotion = animator.GetBool("isRotatingWithRootMotion");
            isInteracting = animator.GetBool("isInteracting");
            canDoCombo = animator.GetBool("canDoCombo");
            canRotate = animator.GetBool("canRotate");
            isInvulnerable = animator.GetBool("isInvulnerable");
            animator.SetBool("isDead", enemyStatsManager.isDead);
            animator.SetBool("isGrabbed", isGrabbed);
        }

        private void LateUpdate() {
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
        }

        // 타겟의 유무와 타겟과의 거리를 통해 현재 행동을 결정한다
        private void HandleStateMachine() {
            if (currentState != null) {
                //if (!enemyStatsManager.isBoss)
                    //Debug.Log(currentState);
                State nextState = currentState.Tick(this, enemyStatsManager, enemyAnimatorManager);
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
            } else currentRecoveryTime = 0;
        }
    }
}
