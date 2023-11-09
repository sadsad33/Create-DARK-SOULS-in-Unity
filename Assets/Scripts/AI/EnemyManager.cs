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

        // Ÿ���� ������ Ÿ�ٰ��� �Ÿ��� ���� ���� �ൿ�� �����Ѵ�
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

        // ���ݰ� ���ݻ����� ������
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

        // ��� ����
        private void AttackTarget() {
            //if (isPerformingAction) return;
            //if (currentAttack == null) {
            //    //Debug.Log("�������� �غ�");
            //    GetNewAttack();
            //} else {
            //    Debug.Log("����!");
            //    isPerformingAction = true;
            //    currentRecoveryTime = currentAttack.recoveryTime;
            //    enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //    currentAttack = null;
            //}
        }

        // ���� ����
        private void GetNewAttack() {
            //Vector3 targetDirection = enemyLocomotionManager.currentTarget.transform.position - transform.position;
            //float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            //enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);
            
            //int maxScore = 0;
            //for (int i = 0; i < enemyAttacks.Length; i++) {
            //    EnemyAttackActions enemyAttackAction = enemyAttacks[i];

            //    if (enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
            //        enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack) { // ���� ��ǥ�� ���� ��Ÿ� ���� �ְ�
            //        if (viewableAngle <= enemyAttackAction.maximumAttackAngle && viewableAngle >= enemyAttackAction.minimumAttackAngle) { // ���� ������ �þ߰����� �ִٸ�
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
            //            if (currentAttack != null) return; // �̹� �������̶��
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
