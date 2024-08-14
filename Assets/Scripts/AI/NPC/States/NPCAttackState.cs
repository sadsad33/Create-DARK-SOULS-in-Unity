using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SoulsLike {
    public class NPCAttackState : State {
        public NPCIdleState npcIdleState;
        public NPCSelectTargetState npcSelectTargetState;
        public NPCPursueTargetState npcPursueTargetState;
        public RotateTowardsTargetState npcRotateTowardsTargetState;
        public EnemyAttackActions currentAttack;

        bool willDoComboOnNextAttack = false;
        public bool hasPerformedAttack = false;
        public override State Tick(AICharacterManager aiCharacter) {
            NPCManager npc = aiCharacter as NPCManager;
            float distanceFromTarget = Vector3.Distance(npc.transform.position, npc.currentTarget.transform.position);

            if (npc.changeTargetTimer <= 0 || npc.currentTarget.characterStatsManager.isDead) {
                Debug.Log("타겟 재설정");
                npc.currentTarget = null;
                return npcIdleState;
            }

            RotateTowardsTargetWhileAttacking(npc);

            if (distanceFromTarget > npc.maximumAggroRadius) return npcPursueTargetState;

            if (willDoComboOnNextAttack && npc.canDoCombo) {
                AttackTargetWithCombo(npc.aiAnimatorManager, npc);
            }

            if (!hasPerformedAttack) {
                AttackTarget(npc.aiAnimatorManager, npc);
            }

            if (willDoComboOnNextAttack && hasPerformedAttack) {
                return this;
            }

            hasPerformedAttack = false;
            return npcRotateTowardsTargetState;
        }

        private void AttackTarget(AICharacterAnimatorManager npcAnimatorManager, NPCManager npcManager) {
            //Debug.Log(npcAnimatorManager.transform.root.gameObject);
            npcAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //npcManager.animator.SetBool("isUsingRightHand", currentAttack.isRightHandedAction);
            //npcManager.animator.SetBool("isUsingLeftHand", !currentAttack.isRightHandedAction);
            //npcManager.UpdateWhichHandCharacterIsUsing(currentAttack.isRightHandedAction);
            npcManager.characterNetworkManager.isUsingRightHand.Value = currentAttack.isRightHandedAction;
            npcManager.characterNetworkManager.isUsingLeftHand.Value = !currentAttack.isRightHandedAction;

            hasPerformedAttack = true;
            npcManager.changeTargetTimer = npcManager.changeTargetTime;
            RollForComboChance(npcManager);
            if (!willDoComboOnNextAttack) {
                npcManager.currentRecoveryTime = currentAttack.recoveryTime;
                currentAttack = null;
            }
        }
        private void AttackTargetWithCombo(AICharacterAnimatorManager npcAnimatorManager, NPCManager npcManager) {
            willDoComboOnNextAttack = false;
            //npcManager.animator.SetBool("isUsingRightHand", currentAttack.isRightHandedAction);
            //npcManager.animator.SetBool("isUsingLeftHand", !currentAttack.isRightHandedAction);
            npcManager.characterNetworkManager.isUsingRightHand.Value = currentAttack.isRightHandedAction;
            npcManager.characterNetworkManager.isUsingLeftHand.Value = !currentAttack.isRightHandedAction;

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
