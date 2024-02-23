using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCPursueTargetState : NPCState {
        public override void Enter(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            Execute(npcManager, npcStatsManager, npcAnimatorManager);
        }

        public override void Execute(NPCManager npcaManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {

        }

        public override NPCState Exit(NPCManager npcaManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            return this;
        }
    }
}