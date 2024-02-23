using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCSelectTargetState : NPCState {
        public NPCIdleState npcIdleState;
        public NPCPursueTargetState npcPursueTargetState;
        float resetValue;
        bool changeTarget;
        public override void Enter(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            resetValue = npcManager.changeTargetTime;
            Execute(npcManager, npcStatsManager, npcAnimatorManager);
        }

        public override void Execute(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            Collider[] colliders = Physics.OverlapSphere(npcManager.transform.position, npcManager.changeTargetDistance, npcManager.currentHostile);
                
            if (npcManager.changeTargetTimer <= 0) {
                
            }
        }

        public override NPCState Exit(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            if (npcManager.currentTarget != null) {
                return npcPursueTargetState;
            } else {
                return npcIdleState;
            }
        }
    }
}