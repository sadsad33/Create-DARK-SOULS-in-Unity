using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoulsLike {
    public class NPCManager : CharacterManager {
        NPCAnimatorManager npcAnimatorManager;
        NPCStatsManager npcStatsManager;
        NPCEffectsManager npcEffectsManager;

        public LayerMask hostileLayer;
        public LayerMask currentHostile;
        public NPCState currentState;
        public Rigidbody npcRigidbody;
        public NavMeshAgent navMeshAgent;

        // 적대 상태가 됐을시 필요
        public List<CharacterStatsManager> targets = new List<CharacterStatsManager>();
        public CharacterStatsManager currentTarget;
        public float rotationSpeed = 15;
        public float maximumAggroRadius = 3f;
        public float attackDistance = 1.5f;

        [Header("AI Settings")]
        public float changeTargetTime = 10;
        public float changeTargetTimer;
        public float detectionRadius = 10;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;

        [Header("AI CombatSettings")]
        public bool allowAIToPerformCombos;
        public bool drawnWeapon;
        public float comboLikelyHood;

        public int interactCount = 0;
        public float aggravationToEnemy;
        public float aggravationToPlayer;

        protected override void Awake() {
            base.Awake();
            canTalk = true;
            npcAnimatorManager = GetComponent<NPCAnimatorManager>();
            npcStatsManager = GetComponent<NPCStatsManager>();
            npcRigidbody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            navMeshAgent.enabled = false;
            navMeshAgent.updateRotation = false;
        }

        private void Start() {
            npcRigidbody.isKinematic = false;
        }

        protected override void Update() {
            HandleStateMachine();
            HandleChangeTargetTimer();
            HandleRecoveryTimer();

            isUsingLeftHand = animator.GetBool("isUsingLeftHand");
            isUsingRightHand = animator.GetBool("isUsingRightHand");
            isRotatingWithRootMotion =animator.GetBool("isRotatingWithRootMotion");
            isInteracting = animator.GetBool("isInteracting");
            canDoCombo = animator.GetBool("canDoCombo");
            canRotate = animator.GetBool("canRotate");
            isInvulnerable = animator.GetBool("isInvulnerable");
            animator.SetBool("isDead", npcStatsManager.isDead);
            animator.SetBool("isGrabbed", isGrabbed);
            if (aggravationToPlayer >= 30 && canTalk) canTalk = false;
        }

        private void LateUpdate() {
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
        }

        private void HandleChangeTargetTimer() {
            if (changeTargetTimer <= 0) {
                //currentTarget = null;
                changeTargetTimer = 0;
            } else {
                //Debug.Log(changeTargetTimer);
                changeTargetTimer -= Time.deltaTime;
            }
        }

        private void HandleStateMachine() {
            if (currentState != null) {
                NPCState nextState = currentState.Tick(this, npcStatsManager, npcAnimatorManager);
                if (nextState != null) {
                    SwitchToNextState(nextState);
                }
            }
        }

        public void SwitchToNextState(NPCState state) {
            //if (currentState != state)
            //    Debug.Log("상태 전이 : " + currentState + " -> " + state);
            currentState = state;
        }

        private void HandleRecoveryTimer() {
            if (currentRecoveryTime > 0) {
                currentRecoveryTime -= Time.deltaTime;
            } else currentRecoveryTime = 0;
        }
    }
}
