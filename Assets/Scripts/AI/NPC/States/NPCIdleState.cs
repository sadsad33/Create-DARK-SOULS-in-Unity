using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class NPCIdleState : NPCState {
        public NPCPursueTargetState npcPursueTargetState;
        public NPCSelectTargetState npcSelectTargetState;
        Collider[] colliders;
        int teamCode;

        public override void Enter(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            Execute(npcManager, npcStatsManager, npcAnimatorManager);
        }

        public override void Execute(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            colliders = Physics.OverlapSphere(npcManager.transform.position, npcManager.detectionRadius, npcManager.hostileLayer);
            for (int i = 0; i < colliders.Length; i++) {
                CharacterStatsManager character = colliders[i].transform.GetComponent<CharacterStatsManager>();
                if (character != null)
                    teamCode = character.teamIDNumber;

                if (teamCode % 2 == 1 && npcManager.aggravationToEnemy >= 30) {
                    npcManager.currentHostile |= LayerMask.GetMask("Character");
                    npcManager.targets.Add(character);
                }
                if (teamCode % 2 == 0 && npcManager.aggravationToPlayer >= 30) {
                    npcManager.currentHostile |= LayerMask.GetMask("Player");
                    npcManager.targets.Add(character);
                }
            }
            Exit(npcManager, npcStatsManager, npcAnimatorManager);
        }

        public override NPCState Exit(NPCManager npcManager, NPCStatsManager npcStatsManager, NPCAnimatorManager npcAnimatorManager) {
            if (npcManager.targets.Count > 0) {
                if (npcManager.targets.Count == 1) return npcPursueTargetState;
                return npcSelectTargetState;
            }
            return this;
        }
    }
}