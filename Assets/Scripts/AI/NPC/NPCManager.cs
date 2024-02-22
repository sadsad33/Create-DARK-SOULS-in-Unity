using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoulsLike {
    public class NPCManager : CharacterManager {
        NPCAnimatorManager npcAnimatorManager;
        NPCStatsManager npcStatsManager;
        NPCEffectsManager npcEffectsManager;

        public State currentState;
        public Rigidbody npcRigidbody;

        // 적대상태 혹은 전투상태가 됐을시 필요
        public CharacterStatsManager currentTarget;
        public NavMeshAgent navMeshAgent;
        public float rotationSpeed = 15;
        public float maximumAggroRadius = 1.5f;

        [Header("AI Settings")]
        public float detectionRadius = 20;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;

        [Header("AI CombatSettings")]
        public bool allowAIToPerformCombos;
        public float comboLikelyHood;

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

        #region 캐릭터 상태제어
        //private void HandleStateMachine() {
        //    if (currentState != null) {
        //        State nextState = currentState.Tick(this, npcStatsManager, npcAnimatorManager);
        //        if (nextState != null) {
        //            SwitchToNextState(nextState);
        //        }
        //    }
        //}

        //private void SwitchToNextState(State state) {
        //    currentState = state;
        //}

        #endregion
    }
}
