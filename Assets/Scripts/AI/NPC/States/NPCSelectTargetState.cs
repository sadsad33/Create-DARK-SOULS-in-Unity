using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCSelectTargetState : NPCState {
        public NPCIdleState npcIdleState;
        public NPCPursueTargetState npcPursueTargetState;
        public override NPCState Tick(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {

            if (npcManager.currentTarget == null) {
                // 현재 주변 적대 관계 오브젝트들과의 거리를 구함
                // 최소 거리에 있는 오브젝트를 목표로 설정
                float shortestPath = Mathf.Infinity;
                for (int i = 0; i < npcManager.targets.Count; i++) {
                    CharacterStatsManager character = npcManager.targets[i].transform.GetComponent<CharacterStatsManager>();
                    if (character != null) {
                        if (character.isDead) continue;

                        float distance = Vector3.Distance(npcManager.transform.position, character.transform.position);
                        if (distance < shortestPath) {
                            shortestPath = distance;
                            npcManager.currentTarget = character;
                        }
                    }
                }
            }
            if (npcManager.currentTarget != null) {
                if (npcManager.currentTarget.isDead) {
                    npcManager.targets.Remove(npcManager.currentTarget);
                    npcManager.currentTarget = null;
                    return npcIdleState;
                }
                
                if (npcManager.changeTargetTimer <= 0)
                    npcManager.changeTargetTimer = npcManager.changeTargetTime;
                return npcPursueTargetState;
            } else {
                return npcIdleState;
            }
        }
    }
}