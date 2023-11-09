using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class EnemyManager : CharacterManager {
        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyStats enemyStats;

        public bool isPerformingAction;
        public State currentState;
        public CharacterStats currentTarget;

        [Header("AI Settings")]
        public float detectionRadius = 20;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;

        private void Awake() {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyStats = GetComponent<EnemyStats>();
        }

        private void Update() {
            HandleRecoveryTimer();
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

        #region Attacks

        // 대상 공격
        private void AttackTarget() {
            //if (isPerformingAction) return;
            //if (currentAttack == null) {
            //    //Debug.Log("다음공격 준비");
            //    GetNewAttack();
            //} else {
            //    Debug.Log("공격!");
            //    isPerformingAction = true;
            //    currentRecoveryTime = currentAttack.recoveryTime;
            //    enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //    currentAttack = null;
            //}
        }

        // 공격 선택
        private void GetNewAttack() {
            //Vector3 targetDirection = enemyLocomotionManager.currentTarget.transform.position - transform.position;
            //float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            //enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);
            
            //int maxScore = 0;
            //for (int i = 0; i < enemyAttacks.Length; i++) {
            //    EnemyAttackActions enemyAttackAction = enemyAttacks[i];

            //    if (enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
            //        enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // 현재 목표가 공격 사거리 내에 있고
            //        if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // 공격 가능한 시야각내에 있다면
            //            maxScore += enemyAttackAction.attackScore;
            //        }
            //    }
            //}

            //int randomValue = Random.Range(0, maxScore);
            //int temporaryScore = 0;
            //for (int i = 0; i < enemyAttacks.Length; i++) {
            //    EnemyAttackActions enemyAttackAction = enemyAttacks[i];

            //    if (enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
            //        enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) {
            //        if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) {
            //            if (currentAttack != null) return; // 이미 공격중이라면
            //            temporaryScore += enemyAttackAction.attackScore;

            //            if (temporaryScore > randomValue) {
            //                currentAttack = enemyAttackAction;
            //            }
            //        }
            //    }
            //}
        }

        #endregion
    }
}
