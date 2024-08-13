using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCPursueTargetState : PursueTargetState {
        public NPCIdleState npcIdleState;
        public NPCSelectTargetState npcSelectTargetState;
        public NPCCombatStanceState npcCombatStanceState;
        public override State Tick(AICharacterManager aiCharacter) {
            NPCManager npc = aiCharacter as NPCManager;
            if (npc.isInteracting) {
                npc.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (npc.changeTargetTimer <= 0 || npc.currentTarget.characterStatsManager.isDead) {
                Debug.Log("타겟 재설정");
                npc.currentTarget = null;
                return npcIdleState;
            }

            HandleRotateTowardsTarget(npc);
            
            float distance = Vector3.Distance(npc.transform.position, npc.currentTarget.transform.position);
            if (distance <= npc.maximumAggroRadius) return npcCombatStanceState;
            else {
                npc.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                return this;
            }
        }
        private void HandleRotateTowardsTarget(NPCManager npcManager) {
            Vector3 targetDirection = npcManager.currentTarget.transform.position - npcManager.transform.position;

            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(npcManager.transform.rotation, tr, npcManager.rotationSpeed * Time.deltaTime);
            npcManager.transform.rotation = targetRotation;

            npcManager.navMeshAgent.enabled = true;
            npcManager.navMeshAgent.SetDestination(npcManager.currentTarget.transform.position);
        }
    }
}