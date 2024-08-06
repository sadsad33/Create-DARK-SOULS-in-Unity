using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCCombatStanceState : NPCState {
        public NPCIdleState npcIdleState;
        public NPCSelectTargetState npcSelectTargetState;
        public NPCAttackState npcAttackState;
        public NPCPursueTargetState npcPursueTargetState;
        public EnemyAttackActions[] npcAttacks;

        protected bool randomDestinationSet = false;
        protected float verticalMovementValue = 0;
        protected float horizontalMovementValue = 0;
        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            float distance = Vector3.Distance(npcManager.transform.position, npcManager.currentTarget.transform.position);
            npcManager.animator.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime);
            npcManager.animator.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);

            if (npcManager.isInteracting) {
                npcManager.animator.SetFloat("Vertical", 0);
                npcManager.animator.SetFloat("Horizontal", 0);
                return this;
            }

            HandleRotateTowardsTarget(npcManager);
            if (distance > npcManager.maximumAggroRadius) return npcPursueTargetState;

            if (npcManager.changeTargetTimer <= 0 || npcManager.currentTarget.characterStatsManager.isDead) {
                Debug.Log("타겟 재설정");
                npcManager.currentTarget = null;
                return npcIdleState;
            }
            
            if (!randomDestinationSet) {
                randomDestinationSet = true;
                DecideCirclingAction(npcAnimatorManager);
            }

            if (npcManager.currentRecoveryTime <= 0 && npcAttackState.currentAttack != null) { // 현재 공격이 선택된 상태라면 공격 상태로 전이
                randomDestinationSet = false;
                return npcAttackState;
            } else {
                GetNewAttack(npcManager);
            }

            return this;
        }

        private void HandleRotateTowardsTarget(NPCManager npcManager) {
            npcManager.navMeshAgent.enabled = true;
            npcManager.navMeshAgent.SetDestination(npcManager.currentTarget.transform.position);
            Vector3 targetDirection = npcManager.currentTarget.transform.position - npcManager.transform.position;
            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(npcManager.transform.rotation, tr, npcManager.rotationSpeed * Time.deltaTime);
            npcManager.transform.rotation = targetRotation;
        }

        protected virtual void GetNewAttack(NPCManager npcManager) {
            Vector3 targetDirection = npcManager.currentTarget.transform.position - npcManager.transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            float distanceFromTarget = Vector3.Distance(npcManager.transform.position, npcManager.currentTarget.transform.position);

            int maxScore = 0;

            for (int i = 0; i < npcAttacks.Length; i++) {
                EnemyAttackActions npcAttackAction = npcAttacks[i];
                if (distanceFromTarget <= npcAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= npcAttackAction.minimumDistanceNeededToAttack) {
                    if (viewableAngle <= npcAttackAction.maximumAttackAngle && viewableAngle >= npcAttackAction.minimumAttackAngle)
                        maxScore += npcAttackAction.attackScore;
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;
            for (int i = 0; i < npcAttacks.Length; i++) {
                EnemyAttackActions npcAttackAction = npcAttacks[i];

                if (distanceFromTarget <= npcAttackAction.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= npcAttackAction.minimumDistanceNeededToAttack) {
                    if (viewableAngle <= npcAttackAction.maximumAttackAngle && viewableAngle >= npcAttackAction.minimumAttackAngle) {
                        if (npcAttackState.currentAttack != null) return;
                        temporaryScore += npcAttackAction.attackScore;
                    }
                    if (temporaryScore > randomValue) {
                        npcAttackState.currentAttack = npcAttackAction;
                    }
                }
            }
        }
        protected void DecideCirclingAction(NPCAnimatorManager npcAnimatorManager) {
            WalkAroundTarget(npcAnimatorManager);
        }

        protected void WalkAroundTarget(NPCAnimatorManager npcAnimatorManager) {
            //verticalMovementValue = 0.5f;

            verticalMovementValue = Random.Range(-1, 1);
            if (verticalMovementValue < 0) verticalMovementValue = 0f;
            else verticalMovementValue = 0.5f;

            horizontalMovementValue = Random.Range(-1, 1);
            if (horizontalMovementValue >= 0)
                horizontalMovementValue = 0.5f;
            else
                horizontalMovementValue = -0.5f;
        }
    }
}