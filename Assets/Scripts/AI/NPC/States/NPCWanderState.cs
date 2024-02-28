using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCWanderState : NPCState {
        public NPCCombatStanceState npcCombatStanceState;

        protected float verticalMovementValue;
        protected float horizontalMovementValue;
        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            npcAnimatorManager.anim.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime);
            npcAnimatorManager.anim.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);
            HandleRotateTowardsTarget(npcManager);
            int randomValue = Random.Range(0, 50);
            
            if (randomValue <= 1)
                return npcCombatStanceState;

            DecideCirclingAction(npcAnimatorManager);
            return this;
        }

        protected void DecideCirclingAction(NPCAnimatorManager npcAnimatorManager) {
            int randNum = Random.Range(0, 101);
            if (randNum <= 40)
                WalkToTarget(npcAnimatorManager);
            else
                WalkSideway(npcAnimatorManager);
        }

        protected void WalkToTarget(NPCAnimatorManager npcAnimatorManager) {
            //Debug.Log("¾ÕÀ¸·Î °È±â");
            verticalMovementValue = 0.5f;
            horizontalMovementValue = Random.Range(-1, 1);
            if (horizontalMovementValue >= 0) {
                horizontalMovementValue = 0.5f;
            } else {
                horizontalMovementValue = -0.5f;
            }
        }

        protected void WalkSideway(NPCAnimatorManager npcAnimatorManager) {
            //Debug.Log("¿·À¸·Î °È±â");
            verticalMovementValue = 0;
            horizontalMovementValue = Random.Range(-1, 1);
            if (horizontalMovementValue >= 0) {
                horizontalMovementValue = 0.5f;
            } else {
                horizontalMovementValue = -0.5f;
            }
        }

        private void HandleRotateTowardsTarget(NPCManager npcManager) {
            npcManager.navMeshAgent.enabled = true;
            npcManager.navMeshAgent.SetDestination(npcManager.currentTarget.transform.position);
            Vector3 targetDirection = npcManager.currentTarget.transform.position - npcManager.transform.position;
            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(npcManager.transform.rotation, tr, npcManager.rotationSpeed * Time.deltaTime);
            npcManager.transform.rotation = targetRotation;
        }
    }
}
