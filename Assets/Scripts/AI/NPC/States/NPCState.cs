using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public abstract class NPCState : MonoBehaviour{
        public abstract void Enter(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager);
        public abstract void Execute(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager);
        public abstract NPCState Exit(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager);
    }
}
