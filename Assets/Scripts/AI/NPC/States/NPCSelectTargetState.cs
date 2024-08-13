using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCSelectTargetState : State {
        public NPCIdleState npcIdleState;
        public NPCPursueTargetState npcPursueTargetState;
        public override State Tick(AICharacterManager aiCharacter) {
            NPCManager npc = aiCharacter as NPCManager;
            if (npc.currentTarget == null) {
                if (npc.targets.Count <= 0) return npcIdleState;

                // 현재 주변 적대 관계 오브젝트들과의 거리를 구함
                // 최소 거리에 있는 오브젝트를 목표로 설정
                float shortestPath = Mathf.Infinity;
                for (int i = 0; i < npc.targets.Count; i++) {
                    CharacterManager character = npc.targets[i];
                    if (character != null) {
                        if (character.characterStatsManager.isDead) continue;

                        float distance = Vector3.Distance(npc.transform.position, character.transform.position);
                        if (distance < shortestPath) {
                            shortestPath = distance;
                            npc.currentTarget = character;
                        }
                    }
                }
            }
            if (npc.currentTarget != null) {
                if (!npc.currentTarget.characterStatsManager.isDead) {
                    if (npc.changeTargetTimer <= 0) npc.changeTargetTimer = npc.changeTargetTime;
                    return npcPursueTargetState;
                } else {
                    npc.targets.Remove(npc.currentTarget);
                    npc.currentTarget = null;
                }
            }
            return npcIdleState;
        }
    }
}