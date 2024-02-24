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

        // 적대상태 혹은 전투상태가 됐을시 필요
        public List<CharacterStatsManager> targets = new List<CharacterStatsManager>();
        public CharacterStatsManager currentTarget;
        public NavMeshAgent navMeshAgent;
        public float rotationSpeed = 15;
        public float maximumAggroRadius = 1.5f;

        [Header("AI Settings")]
        public float changeTargetTime = 8;
        public float changeTargetTimer;
        public float changeTargetDistance = 2;
        public float detectionRadius = 5;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;

        [Header("AI CombatSettings")]
        public bool allowAIToPerformCombos;
        public float comboLikelyHood;

        public float aggravationToEnemy;
        public float aggravationToPlayer;

        protected override void Awake() {
            base.Awake();
            npcAnimatorManager = GetComponent<NPCAnimatorManager>();
            npcStatsManager = GetComponent<NPCStatsManager>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            navMeshAgent.enabled = false;
            npcRigidbody = GetComponent<Rigidbody>();
        }

        private void Start() {
            npcRigidbody.isKinematic = false;
        }

        private void Update() {
            HandleStateMachine();
            HandleChangeTargetTimer();

            isUsingLeftHand = npcAnimatorManager.anim.GetBool("isUsingLeftHand");
            isUsingRightHand = npcAnimatorManager.anim.GetBool("isUsingRightHand");
            isRotatingWithRootMotion = npcAnimatorManager.anim.GetBool("isRotatingWithRootMotion");
            isInteracting = npcAnimatorManager.anim.GetBool("isInteracting");
            canDoCombo = npcAnimatorManager.anim.GetBool("canDoCombo");
            canRotate = npcAnimatorManager.anim.GetBool("canRotate");
            isInvulnerable = npcAnimatorManager.anim.GetBool("isInvulnerable");
            npcAnimatorManager.anim.SetBool("isDead", npcStatsManager.isDead);
            npcAnimatorManager.anim.SetBool("isGrabbed", isGrabbed);
        }

        private void LateUpdate() {
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
        }

        private void HandleChangeTargetTimer() {
            if (changeTargetTimer <= 0)
                changeTargetTimer = 0;
            else
                changeTargetTimer -= Time.deltaTime;
        }

        #region 캐릭터 상태제어
        private void HandleStateMachine() {
            if (currentState != null) {
                NPCState nextState = currentState.Tick(this, npcStatsManager, npcAnimatorManager);
                if (nextState != null) {
                    SwitchToNextState(nextState);
                }
            }
        }

        private void SwitchToNextState(NPCState state) {
            currentState = state;
        }
        #endregion
    }
}
