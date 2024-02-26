using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCRotateTowardsTargetState : NPCState {
        public NPCCombatStanceState npcCombatStanceState;
        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            npcAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            npcAnimatorManager.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

            Vector3 targetDirection = npcManager.currentTarget.transform.position - npcManager.transform.position;

            if (npcManager.isInteracting)
                return this;
            float angle = Vector3.SignedAngle(npcManager.transform.forward, targetDirection, Vector3.up);
            if (angle > 0 && angle <= 120 && !npcManager.isInteracting) {
                npcAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Right", true);
            } else if (angle > 120 && angle <= 180 && !npcManager.isInteracting) {
                npcAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Right", true);
            } else if (angle >= -120 && angle < 0 && !npcManager.isInteracting) {
                npcAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Left", true);
            } else if (angle > -180 && angle < -120 && !npcManager.isInteracting) {
                npcAnimatorManager.PlayTargetAnimationWithRootRotation("Turn_Behind_Left", true);
            }
            return npcCombatStanceState;
        }
    }
}
