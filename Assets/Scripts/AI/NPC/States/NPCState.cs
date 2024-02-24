using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public abstract class NPCState : MonoBehaviour{
        public abstract NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager);
    }
}
