using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoulsLike {
    public class NPCManager : AICharacterManager {

        public LayerMask hostileLayer;
        public LayerMask currentHostile;

        // 적대 상태가 됐을시 필요
        public List<CharacterManager> targets = new List<CharacterManager>();

        [Header("AI Settings")]
        public float changeTargetTime = 10;
        public float changeTargetTimer;

        [Header("AI CombatSettings")]
        public bool hasDrawnWeapon;

        public int interactCount = 0;
        public float aggravationToEnemy;
        public float aggravationToPlayer;

        protected override void Awake() {
            base.Awake();
            canTalk = true;
        }

        protected override void Start() {
            base.Start();
        }

        protected override void Update() {
            base.Update();
            if (aggravationToPlayer >= 30 && canTalk) canTalk = false;
            HandleChangeTargetTimer();
        }

        protected override void LateUpdate() {
            base.LateUpdate();
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

        //protected override void HandleStateMachine() {
        //    base.HandleStateMachine();
        //}

        //public void SwitchToNextState(NPCState state) {
        //    //if (currentState != state)
        //    //    Debug.Log("상태 전이 : " + currentState + " -> " + state);
        //    currentState = state;
        //}

        //private void HandleRecoveryTimer() {
        //    if (currentRecoveryTime > 0) {
        //        currentRecoveryTime -= Time.deltaTime;
        //    } else currentRecoveryTime = 0;
        //}
    }
}
