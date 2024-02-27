using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SoulsLike {
    public class NPCAttackState : NPCState {
        public NPCSelectTargetState npcSelectTargetState;
        public NPCPursueTargetState npcPursueTargetState;
        public NPCRotateTowardsTargetState npcRotateTowardsTargetState;
        public EnemyAttackActions currentAttack;

        bool willDoComboOnNextAttack = false;
        public bool hasPerformedAttack = false;
        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            float distanceFromTarget = Vector3.Distance(npcManager.transform.position, npcManager.currentTarget.transform.position);

            if (npcManager.changeTargetTimer <= 0) return npcSelectTargetState;
            RotateTowardsTargetWhileAttacking(npcManager);
            if (distanceFromTarget > npcManager.maximumAggroRadius) return npcPursueTargetState;

            if (willDoComboOnNextAttack && npcManager.canDoCombo) {
                AttackTargetWithCombo(npcAnimatorManager, npcManager);
            }

            if (!hasPerformedAttack) {
                AttackTarget(npcAnimatorManager, npcManager);
            }

            if (willDoComboOnNextAttack && hasPerformedAttack) {
                return this;
            }
            return npcRotateTowardsTargetState;
        }

        private void AttackTarget(NPCAnimatorManager npcAnimatorManager, NPCManager npcManager) {
            npcAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            npcAnimatorManager.anim.SetBool("isUsingRightHand", currentAttack.isRightHandedAction);
            npcAnimatorManager.anim.SetBool("isUsingLeftHand", !currentAttack.isRightHandedAction);
            hasPerformedAttack = true;
            npcManager.changeTargetTimer = npcManager.changeTargetTime;
            RollForComboChance(npcManager);
            if (!willDoComboOnNextAttack) {
                npcManager.currentRecoveryTime = currentAttack.recoveryTime;
                currentAttack = null;
            }
        }
        private void AttackTargetWithCombo(NPCAnimatorManager npcAnimatorManager, NPCManager npcManager) {
            willDoComboOnNextAttack = false;
            npcAnimatorManager.anim.SetBool("isUsingRightHand", currentAttack.isRightHandedAction);
            npcAnimatorManager.anim.SetBool("isUsingLeftHand", !currentAttack.isRightHandedAction);
            npcAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            npcManager.changeTargetTimer = npcManager.changeTargetTime;
            npcManager.currentRecoveryTime = currentAttack.recoveryTime;
            currentAttack = null;
        }

        private void RotateTowardsTargetWhileAttacking(NPCManager npcManager) {
            if (npcManager.canRotate && npcManager.isInteracting) {
                Vector3 direction = npcManager.currentTarget.transform.position - npcManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                    direction = npcManager.transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                npcManager.transform.rotation = Quaternion.Slerp(npcManager.transform.rotation, targetRotation, npcManager.rotationSpeed / Time.deltaTime);
            }
        }

        private void RollForComboChance(NPCManager npcManager) {
            float comboChance = Random.Range(0, 100);

            if (npcManager.allowAIToPerformCombos && comboChance <= npcManager.comboLikelyHood) {
                if (currentAttack.canCombo) {
                    willDoComboOnNextAttack = true;
                    currentAttack = currentAttack.comboAction;
                } else {
                    willDoComboOnNextAttack = false;
                }
            }
        }
    }
}
