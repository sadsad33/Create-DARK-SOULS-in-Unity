using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCPursueTargetState : NPCState {
        public NPCSelectTargetState npcSelectTargetState;
        public NPCCombatStanceState npcCombatStanceState;
        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            if (npcManager.isInteracting) {
                npcAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (npcManager.changeTargetTimer <= 0) {
                return npcSelectTargetState;
            }

            HandleRotateTowardsTarget(npcManager);

            float distance = Vector3.Distance(npcManager.transform.position, npcManager.currentTarget.transform.position);
            if (distance <= npcManager.maximumAggroRadius) return npcCombatStanceState;
            else {
                npcAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                return this;
            }
        }
        private void HandleRotateTowardsTarget(NPCManager npcManager) {
            Vector3 targetVelocity = npcManager.npcRigidbody.velocity;
            npcManager.navMeshAgent.enabled = true;
            npcManager.navMeshAgent.SetDestination(npcManager.currentTarget.transform.position);
            npcManager.npcRigidbody.velocity = targetVelocity;
            npcManager.transform.rotation = Quaternion.Slerp(npcManager.transform.rotation, npcManager.navMeshAgent.transform.rotation, npcManager.rotationSpeed / Time.deltaTime);
        }
    }
}