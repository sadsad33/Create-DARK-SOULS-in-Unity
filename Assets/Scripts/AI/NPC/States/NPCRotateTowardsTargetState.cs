using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCRotateTowardsTargetState : NPCState {
        public NPCCombatStanceState npcCombatStanceState;
        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            npcManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            npcManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

            Vector3 targetDirection = npcManager.currentTarget.transform.position - npcManager.transform.position;

            if (npcManager.isInteracting)
                return this;
            float angle = Vector3.SignedAngle(npcManager.transform.forward, targetDirection, Vector3.up);
            if (angle >= 30 && angle <= 120 && !npcManager.isInteracting) {
                npcAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Right", true);
                //Debug.Log("Turn_Right");
            } else if (angle > 120 && angle <= 180 && !npcManager.isInteracting) {
                npcAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Right", true);
                //Debug.Log("Turn_Behind_Right");
            } else if (angle >= -120 && angle <= -30 && !npcManager.isInteracting) {
                npcAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Left", true);
                //Debug.Log("Turn_Left");
            } else if (angle > -180 && angle < -120 && !npcManager.isInteracting) {
                npcAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Left", true);
                //Debug.Log("Turn_Behind_Left");
            }
            return npcCombatStanceState;
        }
    }
}
